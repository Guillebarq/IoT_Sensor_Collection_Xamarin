using FIUAssist.DatabaseManager;
using FIUAssist.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIUAssist.Sensors
{
    [Serializable]
    public class SensorActivityWatch
    {

        #region properties of sensors
        public double XAccelerationValue { get; set; }
        public double YAccelerationValue { get; set; }
        public double ZAccelerationValue { get; set; }
        public double XMagnetometerValue { get; set; }
        public double YMagnetometerValue { get; set; }
        public double ZMagnetometerValue { get; set; }
        public double XGyroscopeValue { get; set; }
        public double YGyroscopeValue { get; set; }
        public double ZGyroscopeValue { get; set; }
        public double HeartRateValue { get; set; }
        public double StepCountValue { get; set; }
        public double Proximity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        DateTimeOffset TimeSpan { get; set; }
        String TimeStamp { get; set; }
        #endregion 

        private static SensorActivityWatch _instance;

        private SensorActivityWatch current;

        #region Instance

        public SensorActivityWatch()
        {

        }


        public static SensorActivityWatch Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SensorActivityWatch();
                }
                return _instance;
            }
        }

        public SensorActivityWatch GetSensorActivityWatch()
        {
            if (current == null)
            {
                current = new SensorActivityWatch();
            }

            current.XAccelerationValue = Instance.XAccelerationValue;
            current.YAccelerationValue = Instance.YAccelerationValue;
            current.ZAccelerationValue = Instance.ZAccelerationValue;
            current.XMagnetometerValue = Instance.XMagnetometerValue;
            current.YMagnetometerValue = Instance.YMagnetometerValue;
            current.ZMagnetometerValue = Instance.ZMagnetometerValue;
            current.XGyroscopeValue = Instance.XGyroscopeValue;
            current.YGyroscopeValue = Instance.YGyroscopeValue;
            current.ZGyroscopeValue = Instance.ZGyroscopeValue;
            current.HeartRateValue = Instance.HeartRateValue;
            current.StepCountValue = Instance.StepCountValue;
            current.TimeStamp = DateTime.Now.ToString(Constants.DATETIMEFORMAT);
            //current.TimeStamp = DateTime.Now.ToString(Constants.DATETIMEFORMAT);
            current.Latitude = Instance.Latitude;
            current.Longitude = Instance.Longitude;
            current.Altitude = Instance.Altitude;
            return current;
        }


        static SensorActivityWatch newData = new SensorActivityWatch();
        public static SensorActivityWatch GetSensorActivityWatchFromJson(SensorDataJson watchData)
        {
            try
            {
                newData.XAccelerationValue = watchData.accelerometer_x;
                newData.YAccelerationValue = watchData.accelerometer_y;
                newData.ZAccelerationValue = watchData.accelerometer_z;
                newData.XMagnetometerValue = watchData.magneticfield_x;
                newData.YMagnetometerValue = watchData.magneticfield_y;
                newData.ZMagnetometerValue = watchData.magneticfield_z;
                newData.XGyroscopeValue = watchData.magneticfield_x;
                newData.YGyroscopeValue = watchData.magneticfield_y;
                newData.ZGyroscopeValue = watchData.magneticfield_z;
                newData.HeartRateValue = watchData.heartRate;
                newData.StepCountValue = watchData.stepCount_Value;
                newData.TimeStamp = watchData.TimeStamp;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not convert");
            }
            return newData;
        }

        public static double[] GetDoubleArray(SensorDataJson watchData)
        {
            double[] data = new double[Constants.LENGTH];
            data[Constants.XAccelerationValue] = watchData.accelerometer_x;
            data[Constants.YAccelerationValue] = watchData.accelerometer_x;
            data[Constants.ZAccelerationValue] = watchData.accelerometer_z;
            data[Constants.XMagnetometerValue] = watchData.magneticfield_x;
            data[Constants.YMagnetometerValue] = watchData.magneticfield_y;
            data[Constants.ZMagnetometerValue] = watchData.magneticfield_z;
            data[Constants.XGyroscopeValue] = watchData.gyroscope_x;
            data[Constants.YGyroscopeValue] = watchData.gyroscope_y;
            data[Constants.ZGyroscopeValue] = watchData.gyroscope_z;
            data[Constants.StepCount] = watchData.stepCount_Value;
            data[Constants.HeartRate] = watchData.heartRate;
            return data;
        }




    }
    #endregion
}
