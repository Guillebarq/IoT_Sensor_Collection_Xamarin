using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIUAssist.DatabaseManager
{
    [JsonObject]
    public class SmartWatchSensors
    {
        [JsonProperty(PropertyName = "id")]
        public string mId { get; set; }

        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTimeOffset mCreatedAt { get; set; }

        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTimeOffset mUpdatedAt { get; set; }

        [JsonProperty(PropertyName = "Version")]
        public string mVersion { get; set; }

        [JsonProperty(PropertyName = "accelerometer_acc")]
        public int accelerometer_acc { get; set; }

        [JsonProperty(PropertyName = "accelerometer_x")]
        public float accelerometer_X { get; set; }

        [JsonProperty(PropertyName = "accelerometer_y")]
        public float accelerometer_Y { get; set; }

        [JsonProperty(PropertyName = "accelerometer_z")]
        public float accelerometer_Z { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public String TimeStamp { get; set; }

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
        public float gravity_acc { get; set; }

        [JsonProperty(PropertyName = "gravity_x")]
        public float gravity_x { get; set; }

        [JsonProperty(PropertyName = "gravity_y")]
        public float gravity_y { get; set; }

        [JsonProperty(PropertyName = "gravity_z")]
        public float gravity_z { get; set; }

        [JsonProperty(PropertyName = "heartRate_acc")]
        public int heartRate_acc { get; set; }

        [JsonProperty(PropertyName = "heartRate")]
        public int heartRate { get; set; }

        [JsonProperty(PropertyName = "stepCount_acc")]
        public int stepCount_acc { get; set; }

        [JsonProperty(PropertyName = "stepCount_Value")]
        public int stepCount_Value { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public String user_id { get; set; }

        public bool equals(Object o)
        {
            return ((SmartWatchSensors)o).mId == mId;
        }
        public String toString()
        {
            return "SmartWatchSensorsDTO";
        }

    }
}
