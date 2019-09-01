using FIUAssist.DatabaseManager;
using FIUAssist.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FIUAssist.Services
{
    public class PowerBILiveReport
    {

        public enum DATATYPE
        {
            PHONE,
            WATCH,
            WOUND
        }

        HttpClientHandler handler = new HttpClientHandler();

        //SmartPhoneSensors _phoneData = new SmartPhoneSensors();

        public static Action<string, PowerBILiveReport.DATATYPE> PowerBILive;

        public PowerBILiveReport()
        {
            PowerBILive = PushPhonePowerBIAsync;
        }

        public async void PushPhonePowerBIAsync(string data, DATATYPE datatype)
        {
            HttpClient client = new HttpClient(handler);
            try
            {
                if (datatype == DATATYPE.PHONE)
                {
                    var dataPhone = data;
                    HttpRequestMessage b = new HttpRequestMessage(HttpMethod.Post, Constants.URL);
                    StringContent content = new StringContent(dataPhone, Encoding.UTF8, "application/json");
                    b.Content = content;
                    var result = await client.SendAsync(b);
                    System.Diagnostics.Trace.TraceInformation("Data Sent Phone: " + dataPhone);
                    System.Diagnostics.Trace.TraceInformation("Result Phone: " + result.StatusCode);
                }

                if (datatype == DATATYPE.WATCH)
                {
                    var dataWatch = data;
                    HttpRequestMessage w = new HttpRequestMessage(HttpMethod.Post, Constants.SMARTWATCHURL);
                    StringContent watchContent = new StringContent(dataWatch, Encoding.UTF8, "application/json");
                    w.Content = watchContent;
                    var watchResult = await client.SendAsync(w);
                    System.Diagnostics.Trace.TraceInformation("Data Sent Watch: " + dataWatch);
                    System.Diagnostics.Trace.TraceInformation("Result Watch: " + watchResult.StatusCode);
                }

                if (datatype == DATATYPE.WOUND)
                {
                    var dataWound = data;
                    HttpRequestMessage w = new HttpRequestMessage(HttpMethod.Post, Constants.WOUNDURL);
                    StringContent woundContent = new StringContent(dataWound, Encoding.UTF8, "application/json");
                    w.Content = woundContent;
                    var woundResult = await client.SendAsync(w);
                    System.Diagnostics.Trace.TraceInformation("Data Sent Wound: " + dataWound);
                    System.Diagnostics.Trace.TraceInformation("Result Wond: " + woundResult.StatusCode);
                }

            }
            catch (Exception ex)
            {
                //ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }

        }
    }
}
