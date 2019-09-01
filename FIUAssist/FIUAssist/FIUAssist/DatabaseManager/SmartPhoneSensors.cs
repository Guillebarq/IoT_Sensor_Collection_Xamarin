using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIUAssist.DatabaseManager
{
    [JsonObject]
    public class SmartPhoneSensors
    {

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "createdat")]
        public DateTimeOffset mCreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedat")]
        public DateTimeOffset mUpdatedAt { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string mVersion { get; set; }

        [JsonProperty(PropertyName = "Deleted")]
        public float Deleted { get; set; }

        [JsonProperty(PropertyName = "accelerometer_acc")]
        public float accelerometer_acc { get; set; }

        [JsonProperty(PropertyName = "accelerometer_x")]
        public float accelerometer_x { get; set; }

        [JsonProperty(PropertyName = "accelerometer_y")]
        public float accelerometer_y { get; set; }

        [JsonProperty(PropertyName = "accelerometer_z")]
        public float accelerometer_z { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public string TimeStamp { get; set; }

        [JsonProperty(PropertyName = "magneticfield_acc")]
        public float magneticfield_acc { get; set; }

        [JsonProperty(PropertyName = "magneticfield_x")]
        public float magneticfield_x { get; set; }

        [JsonProperty(PropertyName = "magneticfield_y")]
        public float magneticfield_y { get; set; }

        [JsonProperty(PropertyName = "magneticfield_z")]
        public float magneticfield_z { get; set; }

        [JsonProperty(PropertyName = "gyroscope_acc")]
        public float gyroscope_acc { get; set; }

        [JsonProperty(PropertyName = "gyroscope_x")]
        public float gyroscope_x { get; set; }

        [JsonProperty(PropertyName = "gyroscope_y")]
        public float gyroscope_y { get; set; }

        [JsonProperty(PropertyName = "gyroscope_z")]
        public float gyroscope_z { get; set; }

        [JsonProperty(PropertyName = "gravity_acc")]
        public int gravity_acc { get; set; }

        [JsonProperty(PropertyName = "gravity_x")]
        public float gravity_x { get; set; }

        [JsonProperty(PropertyName = "gravity_y")]
        public float gravity_y { get; set; }

        [JsonProperty(PropertyName = "gravity_z")]
        public float gravity_z { get; set; }

        [JsonProperty(PropertyName = "heartrate_acc")]
        public int heartrate_acc { get; set; }

        [JsonProperty(PropertyName = "heartrate")]
        private int heartRate = 0;
        public int getheartRate() { return heartRate; }
        public void setheartRate(int heartRate) { this.heartRate = heartRate; }

        [JsonProperty(PropertyName = "stepcount_acc")]
        private int stepCount_acc = 0;
        public int getstepCount_acc() { return stepCount_acc; }
        public void setstepCount_acc(int stepCount_acc) { this.stepCount_acc = stepCount_acc; }

        [JsonProperty(PropertyName = "stepcount_value")]
        private int stepCount_Value = 0;
        public int getstepCount_Value() { return stepCount_Value; }
        public void setstepCount_Value(int stepCount_Value) { this.stepCount_Value = stepCount_Value; }

        [JsonProperty(PropertyName = "user_id")]
        private string user_id;
        public string getuser_id() { return user_id; }
        public void setuser_id(string user_id) { this.user_id = user_id; }

        //ADD GPS

        [JsonProperty(PropertyName = "latitude")]
        public float latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public float longitude { get; set; }

        [JsonProperty(PropertyName = "altitude")]
        public float altitude { get; set; }

        [JsonProperty(PropertyName = "bearing")]
        public float bearing { get; set; }

        [JsonProperty(PropertyName = "bearingAccuracy")]
        private float bearingAccuracy = 0;
        public float getBearingAccuracy() { return bearingAccuracy; }
        public void setbearingAccuracy(float bearingAccuracy) { this.bearingAccuracy = bearingAccuracy; }

        [JsonProperty(PropertyName = "GPSaccuracy")]
        public float GPSAccuracy{ get; set;}

        [JsonProperty(PropertyName = "gpsTimeStam")]
        public long gpsTimeStam { get; set; }

        [JsonProperty(PropertyName = "humidity_acc")]
        private float humidity_acc = 0;
        public float getHumidity_acc() { return humidity_acc; }
        public void sethumidity_acc(float humidity_acc) { this.humidity_acc = humidity_acc; }

        [JsonProperty(PropertyName = "humidity")]
        private float humidity = 0;
        public float getHumidity() { return humidity; }
        public void sethumidity(float humidity) { this.humidity = humidity; }

        [JsonProperty(PropertyName = "typeLight_acc")]
        private float typeLight_acc = 0;
        public float getTypeLight_acc() { return typeLight_acc; }
        public void setTypeLight_acc(float typeLight_acc) { this.typeLight_acc = typeLight_acc; }

        [JsonProperty(PropertyName = "typeLightValue")]
        private float typeLightValue = 0;
        public float getTypeLightValue() { return typeLightValue; }
        public void settypeLightValue(float typeLightValue) { this.typeLightValue = typeLightValue; }

        [JsonProperty(PropertyName = "pressure_acc")]
        private float pressure_acc = 0;
        public float getPressure_acc() { return pressure_acc; }
        public void setpressure_acc(float pressure_acc) { this.pressure_acc = pressure_acc; }

        [JsonProperty(PropertyName = "pressure_Value")]
        private float pressure_Value = 0;
        public float getPressure_Value() { return pressure_Value; }
        public void setPressure_Value(float pressure_Value) { this.pressure_Value = pressure_Value; }

        [JsonProperty(PropertyName = "proximity_acc")]
        private float proximity_acc = 0;
        public float getProximity_acc() { return proximity_acc; }
        public void setproximity_acc(float proximity_acc) { this.proximity_acc = proximity_acc; }

        [JsonProperty(PropertyName = "proximity_value")]
        private float proximity_value = 0;
        public float getProximity_value() { return proximity_value; }
        public void setproximity_value(float proximity_value) { this.proximity_value = proximity_value; }

        [JsonProperty(PropertyName = "temperature")]
        private float temperature = 0;
        public float getTemperature() { return temperature; }
        public void settemperature(float temperature) { this.temperature = temperature; }

        [JsonProperty(PropertyName = "temperature_acc")]
        private float temperature_acc = 0;
        public float getTemperature_acc() { return temperature_acc; }
        public void settemperature_acc(float temperature_acc) { this.temperature_acc = temperature_acc; }

        [JsonProperty(PropertyName = "linearAcceleration_acc")]
        private float linearAcceleration_acc = 0;
        public float getLinearAcceleration_acc() { return linearAcceleration_acc; }
        public void setlinearAcceleration_acc(float linearAcceleration_acc) { this.linearAcceleration_acc = linearAcceleration_acc; }

        [JsonProperty(PropertyName = "linearAcceleration_x")]
        private float linearAcceleration_x = 0;
        public float getlinearAcceleration_x() { return linearAcceleration_x; }
        public void setlinearAcceleration_x(float linearAcceleration_x) { this.linearAcceleration_x = linearAcceleration_x; }

        [JsonProperty(PropertyName = "linearAcceleration_y")]
        private float linearAcceleration_y = 0;
        public float getlinearAcceleration_y() { return linearAcceleration_y; }
        public void setlinearAcceleration_y(float linearAcceleration_y) { this.linearAcceleration_y = linearAcceleration_y; }

        [JsonProperty(PropertyName = "linearAcceleration_z")]
        private float linearAcceleration_z = 0;
        public float getlinearAcceleration_z() { return linearAcceleration_z; }
        public void setlinearAcceleration_z(float linearAcceleration_z) { this.linearAcceleration_z = linearAcceleration_z; }

        public bool equals(Object o)
        {
            return ((SmartPhoneSensors)o).id == id;
        }

        public String toString()
        {
            return "SmartWatchSensorsDTO";
        }

    }
}
