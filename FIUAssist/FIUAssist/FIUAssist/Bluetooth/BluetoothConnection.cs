using FIUAssist.DatabaseManager;
using FIUAssist.Messages;
using FIUAssist.Utils;
using FIUAssist.Views;
using Newtonsoft.Json.Linq;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace FIUAssist.Bluetooth
{
    public class BluetoothConnection
    {
        IBluetoothLE ble;
        IAdapter adapter;
        static IDevice device;
        private IService Service;

        //WOUND SENSOR IDs
        Guid het = new Guid("0000fff0-0000-1000-8000-00805f9b34fb");
        Guid char4_uuid = new Guid("0000fff4-0000-1000-8000-00805f9b34fb");
        Guid char1_uuid = new Guid("0000fff1-0000-1000-8000-00805f9b34fb");
        Guid desc = new Guid("00002902-0000-1000-8000-00805f9b34fb");

        byte[] deli = new byte[] { (byte)0x01, (byte)0x00 };
        byte[] deli2 = new byte[] { (byte)0x01 };
        byte[] cmd_string = new byte[] { (byte)0xD7 };

        ICharacteristic char1;
        ICharacteristic char4;

        Thread WoundCalculationThread;
        public Action<WoundSensors> GetUpdate;

        //private bool Connected = false;


        public BluetoothConnection()
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            //woundConnectionThread = new Thread(ConnectKnownDevice);
            //woundConnectionThread.Start();
            //adapter.DeviceDisconnected += DisconnectedSensor;
            adapter.DeviceConnectionLost += WoundSensorDisconnectedAsync;
            adapter.DeviceDiscovered += (s, a) =>
            {
                if (a.Device.Name == "HET Wound v2.1-001")
                {
                    device = a.Device;
                    StaticObjects.Connected = true;
                                      
                    ConnectToWoundSensorAsync(device);
                }
            };
        }

        public async void ConnectKnownDeviceAsync()
        {

            try
            {
                while (!StaticObjects.Connected)
                {
                    await adapter.StartScanningForDevicesAsync();
                }
            }
            catch (DeviceConnectionException ex)
            {
                var message = new ErrorMessage { Message = ex.ToString() };

                Device.BeginInvokeOnMainThread(
                    () => MessagingCenter.Send(message, "ErrorMessage")
                );
            }
        }

        private async void ConnectToWoundSensorAsync(IDevice device)
        {
            try
            {
                //await adapter.StopScanningForDevicesAsync();

                if (device != null && device.Name == "HET Wound v2.1-001")
                {
                    string notif = ("You've connected to " + device.Name);
                    await adapter.ConnectToDeviceAsync(device);

                    Service = await device.GetServiceAsync(het);

                    char4 = await Service.GetCharacteristicAsync(char4_uuid);


                    char1 = await Service.GetCharacteristicAsync(char1_uuid);

                    try
                    {
                        await char1.WriteAsync(cmd_string);
                    }
                    catch
                    {

                    }

                    char4.ValueUpdated += CharacteristicsChanged;

                    await char4.StartUpdatesAsync();

                    //if (SensorDetailsPage.UpdateDeviceName != null)
                    //{
                    //    SensorDetailsPage.UpdateDeviceName(device.Name.ToString());
                    //}

                    //if (SensorDetailsPage.UpdateConnectionStatus != null)
                    //{
                    //    SensorDetailsPage.UpdateConnectionStatus("Connected");
                    //}

                    var message = new NotifyMessage { Message = notif };

                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "NotifyMessage")
                    );

                }
                else
                {

                    var message = new NotifyMessage { Message = "Device not supported !" };

                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "NotifyMessage")
                    );
                }
            }
            catch (DeviceConnectionException ex)
            {
                var message = new NotifyMessage { Message = ex.Message.ToString() };

                Device.BeginInvokeOnMainThread(
                    () => MessagingCenter.Send(message, "NotifyMessage")
                );
            }
        }

        private async void WoundSensorDisconnectedAsync(object sender, DeviceErrorEventArgs e)
        {
            try
            {
                await adapter.StopScanningForDevicesAsync();
                await adapter.DisconnectDeviceAsync(device);
                char4.ValueUpdated -= CharacteristicsChanged;

                //if (SensorDetailsPage.UpdateDeviceName != null)
                //{
                //    SensorDetailsPage.UpdateDeviceName("No Device");
                //}

                //if (SensorDetailsPage.UpdateConnectionStatus != null)
                //{
                //    SensorDetailsPage.UpdateConnectionStatus("Disconnected");
                //}

                var message = new NotifyMessage { Message = "Disconnected from device!" };

                Device.BeginInvokeOnMainThread(
                    () => MessagingCenter.Send(message, "NotifyMessage")
                );

                StaticObjects.Connected = false;

                //if (WoundPage.ConnectionDelete != null)
                //{
                //    WoundPage.ConnectionDelete(Connected);
                //}

                while (!StaticObjects.Connected)
                {
                    try
                    {
                        await adapter.StartScanningForDevicesAsync();
                    }
                    catch (Exception ex)
                    {
                        var _message = new NotifyMessage { Message = "Can't find device" };

                        Device.BeginInvokeOnMainThread(
                            () => MessagingCenter.Send(_message, "NotifyMessage")
                        );

                    }

                }
                //Connected = true;
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }
            //await adapter.DisconnectDeviceAsync(device);
            
        }

        public static Queue<byte[]> data_packets = new Queue<byte[]>();

        public void CharacteristicsChanged(object sender, CharacteristicUpdatedEventArgs e)
        {
            byte[] data = e.Characteristic.Value;
            //DisplayAlert("Notice", data[0].ToString(), "OK");
            if (data != null)
            {
                data_packets.Enqueue(data);
            }

            if (WoundCalculationThread == null)
            {
                WoundCalculationThread = new Thread(ScaleReadyData);
                WoundCalculationThread.Start();
            }

        }

        WoundSensors wound;

        public void ScaleReadyData()
        {
            while (StaticObjects.jobCancelled == false)
            {
                if (data_packets.Count > 0)
                {
                    var _data = data_packets.Dequeue();

                    try
                    {
                        //Stopwatch watch = new Stopwatch();
                        //watch.Start();
                        if (_data != null)
                        {
                            if (wound == null)
                                wound = new WoundSensors();

                            int channel = _data[0];

                            float bias = (float)Math.Round((_data[1] - 1) * -0.05, 1);

                            float voltage = ((_data[2] & 0xff) << 8) | (_data[3] & 0xff);

                            float temp = ((_data[6] & 0xff) << 8) | (_data[7] & 0xff);
                            float batteryOut = (float)(((_data[4] & 0xff) << 8) | (_data[5] & 0xff));
                            float batteryVoltage = (batteryOut) / (float)(435.0);
                            float current = (float)((voltage) / 32767.0 * 2.048);


                            float temperature = (float)Math.Round((temp) / 4096.0 * 3.3, 3);

                            wound.user_id = MainPage.mac;
                            wound.mChannel = channel;
                            wound.mTemperature = temperature;
                            wound.mBattery_voltage = batteryVoltage;
                            wound.mBias = bias;
                            wound.mLmpoutput = current;
                            wound.mTimeStamp = (System.DateTime.Now.ToString(Constants.DATETIMEFORMAT));
                            //woundData = ConvertWoundSensor(woundData, wound);

                            if (WoundPage.SetWoundData != null)
                            {
                                WoundPage.SetWoundData(wound.GetDoubleArray());
                            }


                            SyncData(wound);


                            //if (PowerBILive != null)
                            //{
                            //    var dataWound = string.Format(Constants.wounddata, DateTime.Now.ToString(Constants.DATETIMEFORMAT), woundData.Channel.ToString(), woundData.Lmpoutput.ToString(), woundData.Temperature.ToString());
                            //    PowerBILive(dataWound, NetworkCommunicationManager.DATATYPE.WOUND);
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                    }
                }
            }
        }

        static async void SyncData(WoundSensors data)
        {
            try
            {
                await FIUAssist.DatabaseManager.SensorDataService.Instance.CurrentClient.GetSyncTable("WoundSensors").InsertAsync(JObject.FromObject(data));
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }
        }

    }
}
