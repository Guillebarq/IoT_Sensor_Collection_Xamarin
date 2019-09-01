using Android.Content;
using FIUAssist.DatabaseManager;
using FIUAssist.Messages;
using FIUAssist.Sensors;
using FIUAssist.Utils;
using FIUAssist.Views;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FIUAssist.Services
{
    public class NetworkCommunicationManager
    {
        public static string mac;
        HttpClientHandler handler = new HttpClientHandler();
        public static new Action<PhoneSensorActivity> SendToUI;
        //Thread databaseInsertionThread;
        //public CancellationToken token;
        public static bool isRunning;
        int count = 0;
        public static bool phoneSyncing, woundSyncing, watchSyncing;


        public NetworkCommunicationManager()
        {

        }

        public async Task DBOperationsAsync()
        {
            //while (StaticObjects.jobCancelled == false)
            //{
                try
                {
                    //token.ThrowIfCancellationRequested();

                    var message = new CollectionMessage
                    {
                        Message = "Pushing Data..."
                    };

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send(message, "CollectionMessage");
                    });

                    //INSERTS INTO REMOTE LOCATION
                    await SensorDataService.Instance.CurrentClient.SyncContext.PushAsync();
                    //await SensorDataService.Instance.phoneSensorTable.PurgeAsync(true);
                    //await SensorDataService.Instance.woundSensorTable.PurgeAsync(true);
                    //await SensorDataService.Instance.watchSensorTable.PurgeAsync(true);

                    //if (StaticObjects.path.Length > 8e6)
                    //{
                    //    await SensorDataService.Instance.phoneSensorTable.PurgeAsync(true);
                    //    await SensorDataService.Instance.woundSensorTable.PurgeAsync(true);
                    //    await SensorDataService.Instance.watchSensorTable.PurgeAsync(true);
                    //}

                    var _message = new CollectionMessage
                    {
                        Message = "Push Complete. Waiting..."
                    };
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(_message, "CollectionMessage")
                    );
                    //Thread.Sleep(30000);
                }

                catch (Exception ex)
                {
                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());

                    var _message = new CollectionMessage {Message = "Error Pushing"};
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(_message, "CollectionMessage"));

                    var message = new NotifyMessage { Message = "Error pushing to cloud. The job will restart soon (Maximum 15 minutes)" };
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(_message, "NotifyMessage"));
                }
            //}
        }
    }
}
