using FIUAssist.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FIUAssist.DatabaseManager
{
    [JsonObject]
    public class WoundSensors
    {

        [JsonProperty(PropertyName = "id")]

        public string mId { get; set; }

        //private string mId;
        //public string getId() { return mId; }
        //public void setId(string id) { mId = id; }

        [JsonProperty(PropertyName = "CreatedAt")]
        public DateTimeOffset mCreatedAt { get; set; }
        //private DateTimeOffset mCreatedAt;
        //public DateTimeOffset getCreatedAt() { return mCreatedAt; }
        //protected void setCreatedAt(DateTimeOffset createdAt) { mCreatedAt = createdAt; }

        [JsonProperty(PropertyName = "UpdatedAt")]
        public DateTimeOffset mUpdatedAt { get; set; }
        //private DateTimeOffset mUpdatedAt;
        //public DateTimeOffset getUpdatedAt() { return mUpdatedAt; }
        //protected void setUpdatedAt(DateTimeOffset updatedAt) { mUpdatedAt = updatedAt; }

        [JsonProperty(PropertyName = "Version")]
        public string mVersion { get; set; }
        //private string mVersion;
        //public string getVersion() { return mVersion; }
        //public void setVersion(string version) { mVersion = version; }


        [JsonProperty(PropertyName = "channel")]
        public float mChannel { get; set; }
        //private float mChannel;
        //public float getChannel() { return mChannel; }
        //public void setChannel(float channel) { mChannel = channel; }

        [JsonProperty(PropertyName = "temperature")]
        public float mTemperature { get; set; }
        //private float mTemperature;
        //public float getTemperature() { return mTemperature; }
        //public void setTemperature(float temperature) { mTemperature = temperature; }


        [JsonProperty(PropertyName = "battery_voltage")]
        public float mBattery_voltage { get; set; }
        //private float mBattery_voltage;
        //public float getBattery_voltage() { return mBattery_voltage; }
        //public void setBattery_voltage(float battery_voltage) { mBattery_voltage = battery_voltage; }


        [JsonProperty(PropertyName = "bias")]
        public float mBias { get; set; }
        //private float mBias;
        //public float getBias() { return mBias; }
        //public void setBias(float bias) { mBias = bias; }

        [JsonProperty(PropertyName = "timestamp")]
        public string mTimeStamp { get; set; }
        //public String TimeStamp;
        //public String getTimeStamp() { return TimeStamp; }
        //public void setTimeStamp(String TimeStamp) { this.TimeStamp = TimeStamp; }


        [JsonProperty(PropertyName = "lmpoutput")]
        public float mLmpoutput { get; set; }
        //private float mLmpoutput;
        //public float getLmpoutput() { return mLmpoutput; }
        //public void setLmpoutput(float lmpoutput) { mLmpoutput = lmpoutput; }

        [JsonProperty(PropertyName = "user_id")]
        public string user_id { get; set; }
        //private String user_id;
        //public String getuser_id() { return user_id; }
        //public void setuser_id(String user_id) { this.user_id = user_id; }

        public double[] GetDoubleArray()
        {
            double[] data = new double[Constants.WOUND_LENGTH];
            data[Constants.TEMPERATURE] = this.mTemperature;
            data[Constants.CHANNEL] = this.mChannel;
            data[Constants.BATTERY_VOLTAGE] = this.mBattery_voltage;
            data[Constants.BIAS] = this.mBias;
            data[Constants.LMPOUTPUT] = this.mLmpoutput;
            return data;
        }
    }
}
