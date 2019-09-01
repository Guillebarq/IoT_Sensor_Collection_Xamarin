using FIUAssist.DatabaseManager;
using FIUAssist.Messages;
using FIUAssist.Sensors;
using FIUAssist.Services;
using FIUAssist.Utils;
using FIUAssist.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FIUAssist.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public static Action<double[]> SetPhoneData;
        public static Action<string, string> SetDeviceName;
        public static Action<double[]> StepAndHeartRate;
        public static Action DeleteGraphs;

        private bool startDeletePhone = false;      
        private int countPhone;

        private static Thread phoneSensorsThread;
        public static string mac;

        private ObservableCollection<AccelerationViewModel> accelerationValues;
        private ObservableCollection<MagnetometerViewModel> magnetometerValues;
        private ObservableCollection<GyroscopeViewModel> gyroscopeValues;
        private int heartRateNum;
        private int stepCount;
        private string deviceName;
        string status;

        public static bool DataCollection;
        public static bool UpdateUI = true;
        PollingTimer timer;

        public MainPage()
        {
            InitializeComponent();

            try
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces().Length > 0)
                {
                    //MAC ADDRESS/USER ID FOR PERSON
                    mac = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
                }
            }
            catch
            {

            }


            PowerBILiveReport powerBI = new PowerBILiveReport();

            SetPhoneData = UpdatePhoneData;
            SetDeviceName = SetName;
            StepAndHeartRate = UpdateStepCountAndHeartRate;
            DeleteGraphs = ClearGraphs;

            ExceptionErrorLogger.CopyDatabase();

            AccelerationValues = new ObservableCollection<AccelerationViewModel>();
            MagnetometerValues = new ObservableCollection<MagnetometerViewModel>();
            GyroscopeValues = new ObservableCollection<GyroscopeViewModel>();

            this.BindingContext = this;

            phoneSensorsThread = new Thread(Run);
            phoneSensorsThread.Start();

            DeviceName = "No Device";
            Status = "Disconnected";
            CollectionStat = "....";

            WireUpCollectionMessages();

            HandleReceivedMessages();

            //timer = new PollingTimer(TimeSpan.FromSeconds(30), InitiateCollection);

        }

        private void WireUpCollectionMessages()
        {
            
        }

        public void ClearGraphs()
        {
            gyroscopeValues.Clear();
            accelerationValues.Clear();
            MagnetometerValues.Clear();
        }

        //private void OnTimedEvent(object sender, ElapsedEventArgs e)
        //{
        //    var message = new StartLongRunningTaskMessage();
        //    MessagingCenter.Send(message, "StartLongRunningTaskMessage");          
        //    //CollectionStat = "Collecting Data";
        //}

        string m = "Push Complete";
        private void HandleReceivedMessages()
        {

            MessagingCenter.Subscribe<ErrorMessage>(this, "ErrorMessage", message => {
                Device.BeginInvokeOnMainThread(() => {
                    DisplayAlert("ERROR", message.Message.ToString(), "OK");
                });
            });

            MessagingCenter.Subscribe<NotifyMessage>(this, "NotifyMessage", message => {
                Device.BeginInvokeOnMainThread(() => {

                    DisplayAlert("Notice", message.Message.ToString(), "OK");

                });
            });

            MessagingCenter.Subscribe<CollectionMessage>(this, "CollectionMessage", message => {
                Device.BeginInvokeOnMainThread(() => {
                    //if (message.Message == "Push Complete")
                    //{
                    //    DisplayAlert("Notice", "Data is now stored in database. To store again press the store button.", "OK");
                    //}
                    CollectionStat = message.Message.ToString();
                    //MessagingCenter.Send(message, "StopLongRunningTaskMessage");
                    //DataCollection = false;
                });
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, "CancelledMessage", message => {
                Device.BeginInvokeOnMainThread(() => {
                    CollectionStat = message.ToString();
                });
            });      
        }

        #region MainPage Buttons
        private void BtnStartCollection_Clicked(object sender, EventArgs e)
        {
            
            if (StaticObjects.IsCollecting == false)
            {
                var message = new StartLongRunningTaskMessage();
                MessagingCenter.Send(message, "StartLongRunningTaskMessage");
                //DataCollection = true;
                DisplayAlert("Notice", "Data collection enabled. Storing in database.", "OK");
                //CollectionStat = "Collecting Data";
            }
            else
            {
                DisplayAlert("Notice", "Data already enabled.", "OK");
            }
        }

        //void InitiateCollection()
        //{
        //    if (StaticObjects.IsCollecting == false)
        //    {
        //        var message = new StartLongRunningTaskMessage();
        //        MessagingCenter.Send(message, "StartLongRunningTaskMessage");
        //    }
            
        //}

        //private void Stop_Collection_Clicked(object sender, EventArgs e)
        //{
        //    if (DataCollection == true)
        //    {
        //        var message = new StopLongRunningTaskMessage();
        //        MessagingCenter.Send(message, "StopLongRunningTaskMessage");
        //        DataCollection = false;
        //        DisplayAlert("Notice", "Data collection stopped.", "OK");
        //        CollectionStat = "Not Pushing";
        //    }
        //    else
        //    {
        //        DisplayAlert("Notice", "Data collection already stopped.", "OK");
        //    }
            
        //}

        private async void LogPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InsertionLog());
        }

        private void Stop_UI_Clicked(object sender, EventArgs e)
        {
            if (UpdateUI == true)
            {
                UpdateUI = false;
                accelerationValues.Clear();
                magnetometerValues.Clear();
                gyroscopeValues.Clear();
                countPhone = 0;
                startDeletePhone = false;
                DisplayAlert("Notice", "Graphs disabled.", "OK");
            }
            else
            {
                DisplayAlert("Notice", "Graphs already disabled.", "OK");
            }
            
        }

        private void Start_UI_Clicked(object sender, EventArgs e)
        {
            if (UpdateUI == false)
            {
                UpdateUI = true;
                DisplayAlert("Notice", "Graphs enabled.", "OK");
            }
            else
            {
                DisplayAlert("Notice", "Graphs already enabled.", "OK");
            }
        }

        private void PausePhoneStorage_Clicked(object sender, EventArgs e)
        {

            if (PhoneStore == true)
            {
                PhoneStore = false;
                DisplayAlert("Notice", "No longer storing phone data. UI is still active.", "OK");
            }
            else
            {
                DisplayAlert("Notice", "Already stopped storing phone data", "OK");
            }
            
        }

        private void StartPhoneStorage_Clicked(object sender, EventArgs e)
        {
            if (PhoneStore == false)
            {
                PhoneStore = true;
                DisplayAlert("Notice", "Storing phone data. UI is still active.", "OK");
            }
            else
            {
                DisplayAlert("Notice", "Already storing data.", "OK");
            }
            
        }

        #endregion

        #region Bindable Collections
        public ObservableCollection<AccelerationViewModel> AccelerationValues
        {
            get
            {
                return accelerationValues;
            }

            set
            {
                this.accelerationValues = value;
                OnPropertyChanged("AccelerationValues");
            }
        }

        public ObservableCollection<MagnetometerViewModel> MagnetometerValues
        {
            get
            {
                return magnetometerValues;
            }

            set
            {
                this.magnetometerValues = value;
                OnPropertyChanged("MagnetometerValues");
            }
        }

        public ObservableCollection<GyroscopeViewModel> GyroscopeValues
        {
            get
            {
                return gyroscopeValues;
            }

            set
            {
                this.gyroscopeValues = value;
                OnPropertyChanged("GyroscopeValues");
            }
        }

        public string DeviceName
        {
            get
            {
                return deviceName;
            }
            set
            {
                deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        string collectionStat;

        public string CollectionStat
        {
            get { return collectionStat; }
            set
            {
                collectionStat = value;
                OnPropertyChanged("CollectionStat");
            }
        }

        public int StepCount
        {
            get
            {
                return stepCount;
            }

            set
            {
                this.stepCount = value;
                OnPropertyChanged("StepCount");
            }
        }

        public int HeartRateNum
        {
            get
            {
                return heartRateNum;
            }

            set
            {
                this.heartRateNum = value;
                OnPropertyChanged("HeartRateNum");
            }
        }



        #endregion

        #region UI Update Methods
        void UpdatePhoneData(double[] values)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                double diff = countPhone++;
               

                accelerationValues.Add(new AccelerationViewModel
                {
                    AccelerationTime = diff,
                    XAccelerationValue = values[Constants.XAccelerationValue],
                    YAccelerationValue = values[Constants.YAccelerationValue],
                    ZAccelerationValue = values[Constants.ZAccelerationValue]
                });

                magnetometerValues.Add(new MagnetometerViewModel
                {
                    MagnetometerTime = diff,
                    XMagnetometerValue = values[Constants.XMagnetometerValue],
                    YMagnetometerValue = values[Constants.YMagnetometerValue],
                    ZMagnetometerValue = values[Constants.ZMagnetometerValue]
                });

                gyroscopeValues.Add(new GyroscopeViewModel
                {
                    GyroscopeTime = diff,
                    XGyroscopeValue = values[Constants.XGyroscopeValue],
                    YGyroscopeValue = values[Constants.YGyroscopeValue],
                    ZGyroscopeValue = values[Constants.ZGyroscopeValue]
                });

                try
                {
                    if (startDeletePhone == true)
                    {
                        accelerationValues.RemoveAt(0);
                        magnetometerValues.RemoveAt(0);
                        gyroscopeValues.RemoveAt(0);
                    }
                    if (diff > 20)
                    {
                        if (countPhone > 2)
                        {
                            startDeletePhone = true;
                        }

                        if (countPhone > 500)
                        {
                            accelerationValues.Clear();
                            magnetometerValues.Clear();
                            gyroscopeValues.Clear();
                            countPhone = 0;
                            startDeletePhone = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                }

            });
        }

        void UpdateStepCountAndHeartRate(double[] values)
        {
            
                this.HeartRateNum = (int)values[Constants.HeartRate];
                this.StepCount = (int)values[Constants.StepCount];
            
        }

        #endregion

        #region SQLite Insertion/UI Data

        PhoneSensorActivity objectValue;
        private static bool PhoneStore = true;
        async void Run()
        {
            while (true)
            {
                //token.ThrowIfCancellationRequested();
                objectValue = PhoneSensorActivity.Instance.GetSensorActivityPhone();

                    if (SetPhoneData != null)
                    {
                        SetPhoneData(objectValue.GetDoubleArray());
                    }
                    Thread.Sleep(1000);

                //if (PowerBILiveReport.PowerBILive != null)
                //{
                //    var phone = string.Format(Constants.phoneData, DateTime.Now.ToString(Constants.DATETIMEFORMAT), _phoneData.accelerometer_x.ToString(), _phoneData.accelerometer_y.ToString(), _phoneData.accelerometer_z.ToString());
                //    PowerBILiveReport.PowerBILive("", PowerBILiveReport.DATATYPE.PHONE);
                //}

            }
        }

        public void SetName(string d_name, string d_stat)
        {
            DeviceName = d_name;
            Status = d_stat;
        }
       

        SmartPhoneSensors _phoneData = new SmartPhoneSensors();

        //public async System.Threading.Tasks.Task InsertIntoSQLiteAsync(PhoneSensorActivity obj)
        //{
        //    try
        //    {
        //        var gps = obj.GPSLocationAsync();
        //        _phoneData.mCreatedAt = DateTimeOffset.Now;
        //        _phoneData.accelerometer_x = (float)obj.XAccelerationValue;
        //        _phoneData.accelerometer_y = (float)obj.YAccelerationValue;
        //        _phoneData.accelerometer_z = (float)obj.YAccelerationValue;
        //        _phoneData.accelerometer_acc = (float)obj.AccelerationAverage;
        //        _phoneData.magneticfield_x = (float)obj.XMagnetometerValue;
        //        _phoneData.magneticfield_y = (float)obj.YMagnetometerValue;
        //        _phoneData.magneticfield_z = (float)obj.ZMagnetometerValue;
        //        _phoneData.magneticfield_acc = (float)obj.MagnetometerAverage;
        //        _phoneData.gyroscope_x = (float)obj.XGyroscopeValue;
        //        _phoneData.gyroscope_y = (float)obj.YGyroscopeValue;
        //        _phoneData.gyroscope_z = (float)obj.ZGyroscopeValue;
        //        _phoneData.gyroscope_acc = (float)obj.GyroscopeAverage;
        //        _phoneData.gpsTimeStam = (long)obj.GPSTime;
        //        _phoneData.latitude = (float)obj.Latitude;
        //        _phoneData.longitude = (float)obj.Longitude;
        //        _phoneData.altitude = (float)obj.Altitude;
        //        _phoneData.GPSAccuracy = (float)obj.GPSAccuracy;
        //        _phoneData.TimeStamp = DateTime.Now.ToString(Constants.DATETIMEFORMAT);
        //        _phoneData.setuser_id(mac);
        //        JObject ff = await FIUAssist.DatabaseManager.SensorDataService.Instance.CurrentClient.GetSyncTable("SmartPhoneSensors").InsertAsync(JObject.FromObject(_phoneData));
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}

        #endregion

        #region PollTimer
        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    //Handle action and start your timer
        //    if (StaticObjects.IsCollecting == false)
        //    {
        //        timer.Stop();
        //        InitiateCollection();
        //        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!APPEARING PAGE!!!!!!!!!!!!!!!!!!!!!!!");
        //        timer.Start();
        //    }  
        //}

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    //Stop your timer
        //    //if(StaticObjects.IsCollecting == false) { }
        //    Console.WriteLine("BYEEEEEEEEEEEEEEEEEEEEEEEEEEEEE PAGE!!!!!!!!!!!!!!!!!!!!!!!");
        //    timer.Stop(); //Stop the timer
        //}
        #endregion

    }
}