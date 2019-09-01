using System;
using System.Collections.Generic;
using System.Text;

namespace FIUAssist.Utils
{
    public class Constants
    {
        public const int UPLOAD_TO_AZURE_COUNT_TIMER = 30;
        public const string DATETIMEFORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        public const string CREATED_AT = "dddd, dd MMMM yyyy HH:mm:ss";
        public const string BROADCASTMESSAGE = "Biosen.Service.Activity.Transfer";
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        public const int XAccelerationValue = 0;
        public const int YAccelerationValue = 1;
        public const int ZAccelerationValue = 2;
        public const int AccelerationAverage = 3;
        public const int XMagnetometerValue = 4;
        public const int YMagnetometerValue = 5;
        public const int ZMagnetometerValue = 6;
        public const int MagnetometerAverage = 7;
        public const int XOrientationValue = 8;
        public const int YOrientationValue = 9;
        public const int ZOrientationValue = 10;
        public const int WOrientationValue = 11;
        public const int XGyroscopeValue = 12;
        public const int YGyroscopeValue = 13;
        public const int ZGyroscopeValue = 14;
        public const int GyroscopeAverage = 15;
        public const int Latitude = 16;
        public const int Longitude = 17;
        public const int Altitude = 18;
        public const int GPSAccuracy = 19;
        public const int GPSTime = 20;
        public const int HeartRate = 21;
        public const int StepCount = 22;

        public const string PHONE_ACTION = "PHONE_ACTION";

        public const int LENGTH = 23;

        public const int WOUND_LENGTH = 6;
        public const int CHANNEL = 0;
        public const int TEMPERATURE = 1;
        public const int BATTERY_VOLTAGE = 2;
        public const int BIAS = 3;
        public const int LMPOUTPUT = 4;

        public const string phoneData = "[{{ \"timestamp\":\"{0}\",\"acc_x\":{1},\"acc_y\" :{2},\"acc_z\":{3} }}]";//@"[{{""timestamp"" : {0},""ace_x"" :{1},""ace_y"" :{2},""ace_z"" :{3},""heartrate"" :{4}}}]";
        public const string URL = @"https://api.powerbi.com/beta/ac79e5a8-e0e4-434b-a292-2c89b5c28366/datasets/8aacb0a8-dfee-451f-ae41-5ea76835d02f/rows?key=jyBrIRu4WijCO3GaBply14AXg3i8blktWQSWmNL1WSfOzjyF33mT0CrkdLe2kbJV0LxHL%2F1znJKJ7YziJ4X5Lg%3D%3D";

        public const string smartwatchdata = "[{{ \"timestamp\":\"{0}\",\"acc_x\":{1},\"acc_y\" :{2},\"acc_z\":{3},\"heartrate\":{4},\"stepcount\":{5} }}]";//@"[{{""timestamp"" : {0},""ace_x"" :{1},""ace_y"" :{2},""ace_z"" :{3},""heartrate"" :{4}}}]";
        public const string SMARTWATCHURL = @"https://api.powerbi.com/beta/ac79e5a8-e0e4-434b-a292-2c89b5c28366/datasets/6b463941-7bf6-48c0-9496-a21c23a06a34/rows?key=4t61NKSFitttvkFgp%2FRnUomuKZdwl8r3huKKK6LoT6%2BnTtctl5sOwdAiLiqrP02yXRX8vmDODWLAHfKh8CBBKw%3D%3D";

        public const string wounddata = "[{{ \"timestamp\":\"{0}\",\"channel\":{1},\"lmpout\" :{2},\"temparature\":{3} }}]";//@"[{{""timestamp"" : {0},""ace_x"" :{1},""ace_y"" :{2},""ace_z"" :{3},""heartrate"" :{4}}}]";
        public const string WOUNDURL = @"https://api.powerbi.com/beta/ac79e5a8-e0e4-434b-a292-2c89b5c28366/datasets/b3a724fa-1577-427f-9677-930ff5a2a2d9/rows?key=rixkhhprBL9HvgQ9FZCSyMecOOAg3y68edR55y%2F1K%2B83v%2FAvL3yyW83Zw%2B%2BLf6FLuc2w8loTNveS8C8S%2F5rFvw%3D%3D";

    }
}
