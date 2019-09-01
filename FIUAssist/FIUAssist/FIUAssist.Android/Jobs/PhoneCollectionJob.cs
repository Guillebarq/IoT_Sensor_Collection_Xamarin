using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FIUAssist.DatabaseManager;
using FIUAssist.Sensors;
using FIUAssist.Services;
using FIUAssist.Utils;
using FIUAssist.Views;
using Newtonsoft.Json.Linq;

namespace FIUAssist.Droid.Jobs
{
    [Service(Name = "com.companyname.FIUAssist.Jobs.PhoneCollectionJob", Permission = "android.permission.BIND_JOB_SERVICE")]
    class PhoneCollectionJob : JobService
    {
        //Thread PhoneCollectionThread;
        static Stopwatch s;
        public override bool OnStartJob(JobParameters @params)
        {
            Task.Run( async () =>
            {
                try
                {
                    //var w = new SensorCollectionManager();
                    //PhoneCollectionThread = new Thread(w.PhoneSensorOperationsAsync);
                    //PhoneCollectionThread.Start();
                    s = new Stopwatch();
                    s.Start();
                    PhoneSensorActivity objectValue;
                    while (s.Elapsed < TimeSpan.FromSeconds(900))
                    {
                        
                        objectValue = PhoneSensorActivity.Instance.GetSensorActivityPhone();

                        await InsertIntoSQLiteAsync(objectValue);

                        //if (MainPage.SetPhoneData != null)
                        //{
                        //    MainPage.SetPhoneData(objectValue.GetDoubleArray());
                        //}
                        Console.WriteLine("DATA STORED LOCAL");
                        Thread.Sleep(1000);
                        //}                   
                        //if (PowerBILiveReport.PowerBILive != null)
                        //{
                        //var phone = string.Format(Constants.phoneData, DateTime.Now.ToString(Constants.DATETIMEFORMAT), _phoneData.accelerometer_x.ToString(), _phoneData.accelerometer_y.ToString(), _phoneData.accelerometer_z.ToString());
                        //PowerBILiveReport.PowerBILive("", PowerBILiveReport.DATATYPE.PHONE);
                        //}                      
                    }               
                    s.Stop();
                    s.Reset();
                    JobFinished(@params, false);
                }
                catch (Exception ex)
                {
                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                }

            });

            

            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            //PhoneCollectionThread.Abort();

            //PhoneCollectionThread = null;

            return true;
        }

        SmartPhoneSensors _phoneData = new SmartPhoneSensors();
        public async System.Threading.Tasks.Task InsertIntoSQLiteAsync(PhoneSensorActivity obj)
        {
            try
            {
                await obj.GPSLocationAsync();
                _phoneData.mCreatedAt = DateTimeOffset.Now;
                _phoneData.accelerometer_x = (float)obj.XAccelerationValue;
                _phoneData.accelerometer_y = (float)obj.YAccelerationValue;
                _phoneData.accelerometer_z = (float)obj.YAccelerationValue;
                _phoneData.accelerometer_acc = (float)obj.AccelerationAverage;
                _phoneData.magneticfield_x = (float)obj.XMagnetometerValue;
                _phoneData.magneticfield_y = (float)obj.YMagnetometerValue;
                _phoneData.magneticfield_z = (float)obj.ZMagnetometerValue;
                _phoneData.magneticfield_acc = (float)obj.MagnetometerAverage;
                _phoneData.gyroscope_x = (float)obj.XGyroscopeValue;
                _phoneData.gyroscope_y = (float)obj.YGyroscopeValue;
                _phoneData.gyroscope_z = (float)obj.ZGyroscopeValue;
                _phoneData.gyroscope_acc = (float)obj.GyroscopeAverage;
                _phoneData.gpsTimeStam = (long)obj.GPSTime;
                _phoneData.latitude = (float)obj.Latitude;
                _phoneData.longitude = (float)obj.Longitude;
                _phoneData.altitude = (float)obj.Altitude;
                _phoneData.GPSAccuracy = (float)obj.GPSAccuracy;
                _phoneData.TimeStamp = DateTime.Now.ToString(Constants.DATETIMEFORMAT);
                _phoneData.setuser_id(MainPage.mac);
                await FIUAssist.DatabaseManager.SensorDataService.Instance.CurrentClient.GetSyncTable("SmartPhoneSensors").InsertAsync(JObject.FromObject(_phoneData));
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }

        }
    }
}