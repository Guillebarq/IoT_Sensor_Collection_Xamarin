using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIUAssist.DatabaseManager
{
    public class SensorDataJson
    {
        [JsonProperty(PropertyName = "accelerometer_acc")]
        public int accelerometer_acc = 0;

        [JsonProperty(PropertyName = "accelerometer_x")]
        public float accelerometer_x = 0;

        [JsonProperty(PropertyName = "accelerometer_y")]
        public float accelerometer_y = 0;

        [JsonProperty(PropertyName = "accelerometer_z")]
        public float accelerometer_z = 0;

        [JsonProperty(PropertyName = "timestam")]
        public String TimeStamp;

        [JsonProperty(PropertyName = "magneticfield_acc")]
        public int magneticfield_acc = 0;

        [JsonProperty(PropertyName = "magneticfield_x")]
        public float magneticfield_x = 0;

        [JsonProperty(PropertyName = "magneticfield_y")]
        public float magneticfield_y = 0;

        [JsonProperty(PropertyName = "magneticfield_z")]
        public float magneticfield_z = 0;

        [JsonProperty(PropertyName = "gyroscope_acc")]
        public int gyroscope_acc = 0;

        [JsonProperty(PropertyName = "gyroscope_x")]
        public float gyroscope_x = 0;

        [JsonProperty(PropertyName = "gyroscope_y")]
        public float gyroscope_y = 0;

        [JsonProperty(PropertyName = "gyroscope_z")]
        public float gyroscope_z = 0;

        [JsonProperty(PropertyName = "gravity_acc")]
        public int gravity_acc = 0;

        [JsonProperty(PropertyName = "gravity_x")]
        public float gravity_x = 0;

        [JsonProperty(PropertyName = "gravity_y")]
        public float gravity_y = 0;

        [JsonProperty(PropertyName = "gravity_z")]
        public float gravity_z = 0;

        [JsonProperty(PropertyName = "heartRate_acc")]
        public int heartRate_acc = 0;

        [JsonProperty(PropertyName = "heartRate")]
        public int heartRate = 0;

        [JsonProperty(PropertyName = "stepCount_acc")]
        public int stepCount_acc = 0;

        [JsonProperty(PropertyName = "stepCount_Value")]
        public int stepCount_Value = 0;

        [JsonProperty(PropertyName = "user_id")]
        public String user_id;

    }
}
