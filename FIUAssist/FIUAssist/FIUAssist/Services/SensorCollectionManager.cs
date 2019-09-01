using FIUAssist.DatabaseManager;
using FIUAssist.Sensors;
using FIUAssist.Utils;
using FIUAssist.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FIUAssist.Services
{
    public class SensorCollectionManager
    {

        static Stopwatch s;

        public SensorCollectionManager()
        {
            s = new Stopwatch();
        }


        PhoneSensorActivity objectValue;
        public async void PhoneSensorOperationsAsync()
        {
            s.Start();

            while (s.Elapsed < TimeSpan.FromSeconds(600000))
            {
                objectValue = PhoneSensorActivity.Instance.GetSensorActivityPhone();

                await InsertIntoSQLiteAsync(objectValue);           

                if (MainPage.SetPhoneData != null)
                {
                    MainPage.SetPhoneData(objectValue.GetDoubleArray());
                }
                Console.WriteLine("CONGRAGTULATIONS");
                Thread.Sleep(1000);

                //}

                //if (PowerBILiveReport.PowerBILive != null)
                //{
                //var phone = string.Format(Constants.phoneData, DateTime.Now.ToString(Constants.DATETIMEFORMAT), _phoneData.accelerometer_x.ToString(), _phoneData.accelerometer_y.ToString(), _phoneData.accelerometer_z.ToString());
                //PowerBILiveReport.PowerBILive("", PowerBILiveReport.DATATYPE.PHONE);
                //}                      
            }

            s.Stop();

            //}, token);
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
