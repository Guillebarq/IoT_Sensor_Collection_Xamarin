using Android.Bluetooth;
using Android.Content;
using FIUAssist.DatabaseManager;
using FIUAssist.Sensors;
using FIUAssist.Services;
using FIUAssist.Utils;
using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FIUAssist.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothList : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        ObservableCollection<IDevice> deviceList;
        IDevice device;
        private IList<IService> iServices;
        private IService Service;

        //SMART WATCH ID's 
        Guid watch_uuid = new Guid("0a259db6-2332-11e9-ab14-d663bd873d93");
        Guid[] system_ids;
        private const string Uuid = "0a259db6-2332-11e9-ab14-d663bd873d93";
        //

        //WATCH VARIABLES
        private BluetoothSocket serverClientSocket;
        private Stream inputStream;
        private CancellationToken cancellationToken;
        bool isClosed;
        BluetoothAdapter bluetooth;
        Intent broadCastObject = new Intent();
        Context context;
        private const string ServiceName = "XamarinCookbookBluetoothService";
        //

        //WOUND SENSOR ID's
        Guid het = new Guid("0000fff0-0000-1000-8000-00805f9b34fb");
        Guid char4_uuid = new Guid("0000fff4-0000-1000-8000-00805f9b34fb");
        Guid char1_uuid = new Guid("0000fff1-0000-1000-8000-00805f9b34fb");
        Guid desc = new Guid("00002902-0000-1000-8000-00805f9b34fb");
        //

        //
        byte[] deli = new byte[] { (byte)0x01, (byte)0x00 };
        byte[] deli2 = new byte[] { (byte)0x01 };
        byte[] cmd_string = new byte[] { (byte)0xD7};
        //

        ICharacteristic char1;
        Thread WoundCalculationThread;
        public Action<WoundSensors> GetUpdate;


        public BluetoothList()
        {
            InitializeComponent();
            //BindingContext = this;
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
            DeviceList.ItemsSource = deviceList;
        }

        private async void BtnScan_Clicked(object sender, EventArgs e)
        {
            deviceList.Clear();
            adapter.DeviceDiscovered += (s, a) =>
            {
                var inside = a.Device;
                if (a.Device.Name != null)
                {
                    bool alreadyInside = deviceList.Contains(inside);
                    if (!alreadyInside)
                    {
                        deviceList.Add(inside);
                    }
                    
                }
                
            };
            if (!ble.Adapter.IsScanning)
            {
                await adapter.StartScanningForDevicesAsync();
            }

        }

        //private void BtnStatus_Clicked(object sender, EventArgs e)
        //{
        //    var state = ble.State;
        //    this.DisplayAlert("Notice", state.ToString(), "OK !");
        //    if (state == BluetoothState.Off)
        //    {
        //        btnStatus.TextColor = Color.Red;
        //    }
        //    else
        //    {
        //        btnStatus.TextColor = Color.Green;
        //    }
        //}


        #region HETWound Connection
        private async void BtnConnect_Clicked(object sender, EventArgs e)
        {
            if (device != null)
            {
                switch (device.Name)
                {
                    case "HET Wound v2.1-001":
                        //LOGIC
                        await adapter.StopScanningForDevicesAsync();
                        string notif = ("You've connected to " + device.Name);
                        await adapter.ConnectToDeviceAsync(device);

                        Service = await device.GetServiceAsync(het);

                        var char4 = await Service.GetCharacteristicAsync(char4_uuid);
                        if (MainPage.SetDeviceName != null)
                        {
                            MainPage.SetDeviceName(device.Name, "Connected");
                        }

                        char1 = await Service.GetCharacteristicAsync(char1_uuid);

                        try
                        {
                            await char1.WriteAsync(cmd_string);
                        }
                        catch
                        {

                        }

                        char4.ValueUpdated += CharacteristicsChanged;

                        btnConnect.TextColor = Color.Green;

                        await char4.StartUpdatesAsync();

                        await DisplayAlert("Notice", notif, "OK");
                        break;

                    case "Ticwatch E C36B":
                        //LOGIC
                        await adapter.StopScanningForDevicesAsync();
                        string notif_w = ("You've connected to " + device.Name);
                        //await adapter.ConnectToDeviceAsync(device);
                        try
                        {
                            // set up the server, start and stop listening for new connections
                            BluetoothServerSocket serverSocket = null;
                            // start listening for a new connection
                            if (!isClosed)
                            {
                                try
                                {
                                    bluetooth = BluetoothAdapter.DefaultAdapter;
                                    if (bluetooth == null)
                                    {
                                        Console.WriteLine("NO ADAPTER");
                                    }
                                    // MY_UUID is the app's UUID string, also used by the client code
                                    Console.WriteLine("Starting the server listener");
                                    serverSocket = bluetooth.ListenUsingRfcommWithServiceRecord(ServiceName, UUID.FromString(Uuid));

                                    // keep trying to find a client

                                    serverClientSocket = await serverSocket.AcceptAsync();
                                }
                                catch (IOException ex)
                                {
                                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                                    await DisplayAlert("Notice", "Cannot connect to device. Please try again.", "OK");
                                    return;
                                }
                                // If a connection was accepted
                                if (serverClientSocket != null)
                                {
                                    var device = serverClientSocket.RemoteDevice;

                                    bluetooth = null;
                                    serverSocket.Close();
                                    // Do work to manage the connection (in a separate thread)                                   
                                    try
                                    {

                                        await DisplayAlert("Notice", notif_w, "OK");
                                        btnConnect.TextColor = Color.Green;
                                        await StartListening(serverClientSocket);
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                                        await DisplayAlert("Notice", "Error connecting to Ticwatch. Please try again.", "OK");

                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                        }
                        break;

                    default:
                        await DisplayAlert("Notice", "Device not supported !", "OK");
                        btnConnect.TextColor = Color.Red;
                        break;

                }
            }
            else
            {
                await DisplayAlert("Notice", "No Device Selected!", "OK");
                btnConnect.TextColor = Color.Red;
            }
            


            //try
            //{
            //    if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            //    {
            //        await adapter.StopScanningForDevicesAsync();
            //    }
            //    if (device != null && device.Name == "HET Wound v2.1-001")
            //    {
            //        string notif = ("You've connected to " + device.Name);
            //        await adapter.ConnectToDeviceAsync(device);
                   
            //        Service = await device.GetServiceAsync(het);
                   
            //        var char4 = await Service.GetCharacteristicAsync(char4_uuid);
            //        if (MainPage.SetDeviceName != null)
            //        {
            //            MainPage.SetDeviceName(device.Name, "Connected");
            //        }

            //        char1 = await Service.GetCharacteristicAsync(char1_uuid);

            //        try
            //        {
            //            await char1.WriteAsync(cmd_string);
            //        }
            //        catch
            //        {

            //        }

            //        char4.ValueUpdated += CharacteristicsChanged;

            //        btnConnect.TextColor = Color.Green;

            //        await char4.StartUpdatesAsync();

            //        await DisplayAlert("Notice", notif, "OK");

            //    }
            //    else
            //    {
            //        await DisplayAlert("Notice", "Device not supported !", "OK");
            //        btnConnect.TextColor = Color.Red;
            //    }
            //}
            //catch(DeviceConnectionException ex)
            //{
            //   await  DisplayAlert("Notice", ex.Message.ToString(), "OK");
            //    btnConnect.TextColor = Color.Red;
            //}

        }
        #endregion

        private void BtnPairedDevices_Clicked(object sender, EventArgs e)
        {
            deviceList.Clear();
            system_ids = new Guid[] { watch_uuid };
            foreach (IDevice dev in adapter.GetSystemConnectedOrPairedDevices(system_ids))
            {
                if (dev.Name != null)
                {
                    bool alreadyInside = deviceList.Contains(dev);
                    if (!alreadyInside)
                    {
                        deviceList.Add(dev);
                    }
                }
            }
        }

        private void BtnWatchConnect_Clicked(object sender, EventArgs e)
        {

        }


        private async void BtnConnectKnown_Clicked(object sender, EventArgs e)
        {
            try
            {
                await adapter.ConnectToKnownDeviceAsync(het);
                string notif = ("You've connected to " + device.Name);
                await DisplayAlert("Notice", notif, "OK");
            }
            catch (DeviceConnectionException ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                await DisplayAlert("Notice", ex.Message.ToString(), "OK");
            }
        }

        private void ListItem_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (DeviceList.SelectedItem == null)
            {
                return;
            }
            device = DeviceList.SelectedItem as IDevice;
        }

        private void BtnClearList_Clicked(object sender, EventArgs e)
        {
            if (deviceList != null)
            {
                deviceList.Clear();
            }
        }

        private async void BtnDisconnect_Clicked(object sender, EventArgs e)
        {           
            if (device != null && device.Name == "HET Wound v2.1-001")
            {
                await adapter.DisconnectDeviceAsync(device);
                string notif = ("You've disconnected from " + device.Name);
                await DisplayAlert("Notice", notif, "OK");
                if (WoundPage.ConnectionDelete != null)
                {
                    WoundPage.ConnectionDelete(false);
                }
                if (MainPage.SetDeviceName != null)
                {
                    MainPage.SetDeviceName("No Device", "Disconnected");
                }
                device = null;
            }
            else
            {
               await DisplayAlert("Notice", "You are not connected to a device", "OK");
            }
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
            while (true)
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
                JObject ff = await FIUAssist.DatabaseManager.SensorDataService.Instance.CurrentClient.GetSyncTable("WoundSensors").InsertAsync(JObject.FromObject(data));
                ff = null;
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }
        }

        SmartWatchSensors smartWatchSensors;
        private async Task StartListening(BluetoothSocket socket)
        {
            smartWatchSensors = new SmartWatchSensors();
            // Get the BluetoothSocket input and output streams
            try
            {
                inputStream = socket.InputStream;
                //ConnectionStates.watchConnected = true;
            }
            catch (IOException ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                resetConnection();
            }

            const int BUFFER_SIZE = 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytes = 0;
            int b = BUFFER_SIZE;

            // Keep listening to the InputStream while connected
            while (true)
            {
                try
                {
                    // Read from the input stream
                    bool available = inputStream.IsDataAvailable();

                    if (available)
                    {
                        if (serverClientSocket == null || !serverClientSocket.IsConnected)
                        {
                            resetConnection();
                            break;
                        }
                        int i = 0;
                        while (serverClientSocket != null)
                        {
                            var size = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                            i = 0;
                            if (size > 0)
                            {
                                while (size > i)
                                {
                                    if (buffer[i] == '$')
                                    {
                                        if (buffer[i + 1] == '~' && buffer[i + 2] == '!')
                                        {
                                            resetConnection();
                                            return;
                                        }
                                        int length = buffer[i + 1] << 24 | (buffer[i + 2] & 0xFF) << 16 | (buffer[i + 3] & 0xFF) << 8 | (buffer[i + 4] & 0xFF);
                                        var stringRead = Encoding.UTF8.GetString(buffer, i + 5, i + length + 5);
                                        var startIndex = stringRead.IndexOf("{");
                                        var endIndex = stringRead.IndexOf("}") + 1;
                                        try
                                        {
                                            var response = stringRead.Remove(endIndex, stringRead.Length - endIndex).Remove(0, startIndex);
                                            var jsonObject = JsonConvert.DeserializeObject<SensorDataJson>(response);
                                            var watchData = SensorActivityWatch.GetSensorActivityWatchFromJson(jsonObject);
                                            if (SmartWatchPage.SetWatchData != null)
                                            {
                                                SmartWatchPage.SetWatchData(SensorActivityWatch.GetDoubleArray(jsonObject));
                                            }
                                            if (MainPage.StepAndHeartRate != null)
                                            {
                                                MainPage.StepAndHeartRate(SensorActivityWatch.GetDoubleArray(jsonObject));
                                            }

                                            smartWatchSensors = ConvertJsonToSmartWatchSensor(jsonObject, smartWatchSensors);
                                            await FIUAssist.DatabaseManager.SensorDataService.Instance.CurrentClient.GetSyncTable("SmartWatchSensors").InsertAsync(JObject.FromObject(smartWatchSensors));

                                            //var dataWatch = string.Format(Constants.smartwatchdata, DateTime.Now.ToString(Constants.DATETIMEFORMAT), watchData.XAccelerationValue.ToString(), watchData.YAccelerationValue.ToString(), watchData.ZAccelerationValue.ToString(), watchData.HeartRateValue.ToString(), watchData.StepCountValue.ToString());
                                            //if (PowerBILive != null)
                                            //{
                                            //    PowerBILive(dataWatch, NetworkCommunicationManager.DATATYPE.WATCH);
                                            //}

                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                                            resetConnection();
                                        }
                                        i += length + 4;
                                    }
                                    i += size;
                                }

                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                    resetConnection();
                    return;
                }
            }
        }


        public SmartWatchSensors ConvertJsonToSmartWatchSensor(SensorDataJson jsonData, SmartWatchSensors sensorData)
        {
            sensorData.accelerometer_acc = jsonData.accelerometer_acc;
            sensorData.accelerometer_X = jsonData.accelerometer_x;
            sensorData.accelerometer_Y = jsonData.accelerometer_y;
            sensorData.accelerometer_Z = jsonData.accelerometer_z;

            sensorData.magneticfield_acc = jsonData.magneticfield_acc;
            sensorData.magneticfield_x = jsonData.magneticfield_x;
            sensorData.magneticfield_y = jsonData.magneticfield_y;
            sensorData.magneticfield_z = jsonData.magneticfield_z;

            sensorData.gravity_acc = jsonData.gravity_acc;
            sensorData.gravity_x = jsonData.gravity_x;
            sensorData.gravity_y = jsonData.gravity_y;
            sensorData.gravity_z = jsonData.gravity_y;

            sensorData.gyroscope_acc = jsonData.gyroscope_acc;
            sensorData.gyroscope_x = jsonData.gyroscope_x;
            sensorData.gyroscope_y = jsonData.gyroscope_y;
            sensorData.gyroscope_z = jsonData.gyroscope_z;

            sensorData.heartRate_acc = jsonData.heartRate_acc;
            sensorData.heartRate = jsonData.heartRate;
            sensorData.stepCount_acc = jsonData.stepCount_acc;
            sensorData.stepCount_Value = jsonData.stepCount_Value;

            sensorData.TimeStamp = jsonData.TimeStamp;

            return sensorData;
        }

        public async void resetConnection()
        {
            await DisplayAlert("Notice", "Disconnected from Ticwatch", "OK");
            try
            {
                if (SmartWatchPage.ClearGraphs != null)
                {
                    SmartWatchPage.ClearGraphs();
                }
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }
            try
            {
                if (inputStream != null)
                {
                    inputStream.Close();
                    inputStream = null;
                }
            }
            catch (IOException ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }

            try
            {
                if (serverClientSocket != null)
                {
                    serverClientSocket.Close();
                    serverClientSocket = null;
                }



            }
            catch (IOException ex)
            {
               ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }
        }
    }
}