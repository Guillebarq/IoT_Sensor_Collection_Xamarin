using FIUAssist.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FIUAssist.Sensors
{
    [Serializable]
    public class PhoneSensorActivity
    {
        static string TAG = typeof(PhoneSensorActivity).FullName;
        private PhoneSensorActivity current;
        private static PhoneSensorActivity _instance;

        public static PhoneSensorActivity Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PhoneSensorActivity();
                    _instance.StartSensors();
                }
                return _instance;
            }
        }

        public PhoneSensorActivity()
        {

        }

        public void StartSensors()
        {
            try
            {
                Accelerometer.Start(SensorSpeed.Default);
                Magnetometer.Start(SensorSpeed.Default);
                Gyroscope.Start(SensorSpeed.Default);
                OrientationSensor.Start(SensorSpeed.Default);
                Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
                Gyroscope.ReadingChanged += Gyrotometer_ReadingChanged;
                OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;

            }
            catch (Exception EX)
            {
                //Log.Error(TAG, EX.GetBaseException().ToString());
            }
        }

        public void StopSensors()
        {
            Accelerometer.Stop();
            Magnetometer.Stop();
            Gyroscope.Stop();
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
            Magnetometer.ReadingChanged -= Magnetometer_ReadingChanged;
            Gyroscope.ReadingChanged -= Gyrotometer_ReadingChanged;
            OrientationSensor.ReadingChanged -= OrientationSensor_ReadingChanged;
            _instance = null;
        }
        public async Task<Location> GPSLocationAsync()
        {
            try
            {

                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    return null;
                }
                else
                {
                    this.GPSTime = location.Timestamp.UtcTicks;
                    this.Latitude = location.Latitude;
                    this.Longitude = location.Longitude;
                    var alt = (double)location.Altitude;
                    this.Altitude = alt = Math.Round((float)alt * 3.28084, 0); // convert to feet
                    this.GPSAccuracy = (double)location.Accuracy;
                    return location;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("No gps available");
                return null;
            }

        }

        private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            XOrientationValue = data.Orientation.X;
            YOrientationValue = data.Orientation.Y;
            ZOrientationValue = data.Orientation.Z;
            WOrientationValue = data.Orientation.W;
        }

        private void Gyrotometer_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;
            XGyroscopeValue = data.AngularVelocity.X;
            YGyroscopeValue = data.AngularVelocity.Y;
            ZGyroscopeValue = data.AngularVelocity.Z;
            GyroscopeAverage = Math.Sqrt(Math.Abs((data.AngularVelocity.X * data.AngularVelocity.X) + (data.AngularVelocity.Y * data.AngularVelocity.Y) + (data.AngularVelocity.Z * data.AngularVelocity.Z)));
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            var data = e.Reading;
            XMagnetometerValue = data.MagneticField.X;
            YMagnetometerValue = data.MagneticField.Y;
            ZMagnetometerValue = data.MagneticField.Z;
            MagnetometerAverage = Math.Sqrt(Math.Abs((data.MagneticField.X * data.MagneticField.X) + (data.MagneticField.Y * data.MagneticField.Y) + (data.MagneticField.Z * data.MagneticField.Z)));
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            XAccelerationValue = data.Acceleration.X;
            YAccelerationValue = data.Acceleration.Y;
            ZAccelerationValue = data.Acceleration.Z;
            AccelerationAverage = Math.Sqrt(Math.Abs((data.Acceleration.X * data.Acceleration.X) + (data.Acceleration.Y * data.Acceleration.Y) + (data.Acceleration.Z * data.Acceleration.Z)));
        }


        public PhoneSensorActivity GetSensorActivityPhone()
        {
            if (current == null)
            {
                current = new PhoneSensorActivity();
            }

            current.XAccelerationValue = Instance.XAccelerationValue;
            current.YAccelerationValue = Instance.YAccelerationValue;
            current.ZAccelerationValue = Instance.ZAccelerationValue;
            current.AccelerationAverage = Instance.AccelerationAverage;
            current.XMagnetometerValue = Instance.XMagnetometerValue;
            current.YMagnetometerValue = Instance.YMagnetometerValue;
            current.ZMagnetometerValue = Instance.ZMagnetometerValue;
            current.MagnetometerAverage = Instance.MagnetometerAverage;
            current.XGyroscopeValue = Instance.XGyroscopeValue;
            current.YGyroscopeValue = Instance.YGyroscopeValue;
            current.ZGyroscopeValue = Instance.ZGyroscopeValue;
            current.GyroscopeAverage = Instance.GyroscopeAverage;
            current.XOrientationValue = Instance.XOrientationValue;
            current.YOrientationValue = Instance.YOrientationValue;
            current.ZOrientationValue = Instance.ZOrientationValue;
            current.WOrientationValue = Instance.WOrientationValue;
            current.TimeStamp = DateTime.Now.ToString(Constants.DATETIMEFORMAT);
            current.GPSTime = Instance.GPSTime;
            current.Latitude = Instance.Latitude;
            current.Longitude = Instance.Longitude;
            current.Altitude = Instance.Altitude;
            current.GPSAccuracy = Instance.GPSAccuracy;
            return current;
        }

        public double[] GetDoubleArray()
        {
            double[] data = new double[Constants.LENGTH];
            data[Constants.XAccelerationValue] = Instance.XAccelerationValue;
            data[Constants.YAccelerationValue] = Instance.YAccelerationValue;
            data[Constants.ZAccelerationValue] = Instance.ZAccelerationValue;
            data[Constants.AccelerationAverage] = Instance.AccelerationAverage;
            data[Constants.XMagnetometerValue] = Instance.XMagnetometerValue;
            data[Constants.YMagnetometerValue] = Instance.YMagnetometerValue;
            data[Constants.ZMagnetometerValue] = Instance.ZMagnetometerValue;
            data[Constants.MagnetometerAverage] = Instance.MagnetometerAverage;
            data[Constants.XGyroscopeValue] = Instance.XGyroscopeValue;
            data[Constants.YGyroscopeValue] = Instance.YGyroscopeValue;
            data[Constants.ZGyroscopeValue] = Instance.ZGyroscopeValue;
            data[Constants.GyroscopeAverage] = Instance.GyroscopeAverage;
            data[Constants.XOrientationValue] = Instance.XOrientationValue;
            data[Constants.YOrientationValue] = Instance.YOrientationValue;
            data[Constants.ZOrientationValue] = Instance.ZOrientationValue;
            data[Constants.WOrientationValue] = Instance.WOrientationValue;
            data[Constants.Latitude] = Instance.Latitude;
            data[Constants.Longitude] = Instance.Longitude;
            data[Constants.Altitude] = Instance.Altitude;
            data[Constants.GPSAccuracy] = Instance.GPSAccuracy;
            data[Constants.GPSTime] = Instance.GPSTime;

            return data;
        }


        #region properties of sensors
        public double XAccelerationValue { get; set; }
        public double YAccelerationValue { get; set; }
        public double ZAccelerationValue { get; set; }
        public double AccelerationAverage { get; set; }
        public double XMagnetometerValue { get; set; }
        public double YMagnetometerValue { get; set; }
        public double ZMagnetometerValue { get; set; }
        public double MagnetometerAverage { get; set; }
        public double XGyroscopeValue { get; set; }
        public double YGyroscopeValue { get; set; }
        public double ZGyroscopeValue { get; set; }
        public double GyroscopeAverage { get; set; }
        public double XOrientationValue { get; set; }
        public double YOrientationValue { get; set; }
        public double ZOrientationValue { get; set; }
        public double WOrientationValue { get; set; }
        public double Proximity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double GPSAccuracy { get; set; }
        public double GPSTime { get; set; }

        string TimeStamp { get; set; }
        #endregion 
    }
}
