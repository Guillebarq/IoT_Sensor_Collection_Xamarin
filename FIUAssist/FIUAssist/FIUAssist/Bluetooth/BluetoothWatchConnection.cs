using Android.Bluetooth;
using Android.Content;
using FIUAssist.DatabaseManager;
using FIUAssist.Messages;
using FIUAssist.Sensors;
using FIUAssist.Utils;
using FIUAssist.Views;
using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FIUAssist.Bluetooth
{

    public class BluetoothWatchConnection
    {

        //SMART WATCH ID's 
        Guid watch_uuid = new Guid("0a259db6-2332-11e9-ab14-d663bd873d93");
        Guid[] system_ids;
        private const string Uuid = "0a259db6-2332-11e9-ab14-d663bd873d93";
        //
        IBluetoothLE ble;
        IAdapter adapter;

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


        public BluetoothWatchConnection()
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            cancellationToken = new CancellationToken();
        }

        BluetoothServerSocket serverSocket;

        public static bool Connected;
        public async void ConnectToWatchAsync()
        {
            isClosed = false;
            system_ids = new Guid[] { watch_uuid };
            foreach (IDevice dev in adapter.GetSystemConnectedOrPairedDevices(system_ids))
            {
                if (dev.Name == "Ticwatch E C36B")
                {
                    try
                    {
                        // set up the server, start and stop listening for new connections
                        serverSocket = null;
                        // start listening for a new connection
                        if (!isClosed)
                        {
                            try
                            {
                                bluetooth = BluetoothAdapter.DefaultAdapter;
                                if (bluetooth == null)
                                {
                                    var message = new NotifyMessage { Message = "Bluetooth turned off or not available"};
                                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, "NotifyMessage"));
                                }
                                
                                // MY_UUID is the app's UUID string, also used by the client code
                                Console.WriteLine("Starting the server listener");
                                serverSocket = bluetooth.ListenUsingRfcommWithServiceRecord(ServiceName, UUID.FromString(Uuid));

                                // keep trying to find a client
                                while (serverClientSocket == null)
                                {
                                    serverClientSocket = await serverSocket.AcceptAsync();
                                }

                            }
                            catch (IOException ex)
                            {
                                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                                //await DisplayAlert("Notice", "Cannot connect to device. Please try again.", "OK");
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

                                    //await DisplayAlert("Notice", notif_w, "OK");
                                    isClosed = true;
                                    StartListeningAsync(serverClientSocket, cancellationToken);
                                }
                                catch (Exception ex)
                                {
                                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                                    //await DisplayAlert("Notice", "Error connecting to Ticwatch. Please try again.", "OK");

                                }
                            }
                        }
                        else
                        {
                            if (serverClientSocket != null)
                            {

                                serverSocket = bluetooth.ListenUsingRfcommWithServiceRecord(ServiceName, UUID.FromString(Uuid));

                                var device = serverClientSocket.RemoteDevice;

                                bluetooth = null;
                                serverSocket.Close();
                                // Do work to manage the connection (in a separate thread)                                   
                                try
                                {

                                    //await DisplayAlert("Notice", notif_w, "OK");

                                    StartListeningAsync(serverClientSocket, cancellationToken);
                                }
                                catch (Exception ex)
                                {
                                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                                    //await DisplayAlert("Notice", "Error connecting to Ticwatch. Please try again.", "OK");

                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                    }
                }
            }
        }


        SmartWatchSensors smartWatchSensors;
        Stopwatch s;
        private async void StartListeningAsync(BluetoothSocket socket, CancellationToken cancellationToken)
        {
            smartWatchSensors = new SmartWatchSensors();
            // Get the BluetoothSocket input and output streams
            s = new Stopwatch();
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
                        s.Start();
                        while (serverClientSocket != null && s.Elapsed < TimeSpan.FromSeconds(600000))
                        {
                            var size = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                            i = 0;
                            if (size > 0)
                            {
                                while (size > i && StaticObjects.jobCancelled == false)
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
                        s.Stop();
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

        public void resetConnection()
        {
            //await DisplayAlert("Notice", "Disconnected from Ticwatch", "OK");
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

            ConnectToWatchAsync();
        }

    }
}
