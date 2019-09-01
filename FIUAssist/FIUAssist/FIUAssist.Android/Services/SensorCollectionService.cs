using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FIUAssist.Messages;
using FIUAssist.Services;
using FIUAssist.Utils;
using Xamarin.Forms;

namespace FIUAssist.Droid.Services
{
    [Service]
    class SensorCollectionService: Service
    {

        CancellationTokenSource _cts;

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }


        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                _cts = new CancellationTokenSource();

                String NOTIFICATION_CHANNEL_ID = "com.companyname.FIUAssist";
                string channelName = "My Background Service";
                NotificationChannel chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.High);
                chan.EnableVibration(true);
                chan.Description = "Collecting/Storing sensor data and pushing to Azure Database.";
                chan.EnableLights(true);
                chan.LightColor = 255;
                chan.LockscreenVisibility = NotificationVisibility.Public;
                NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService);
                if (manager != null)
                    manager.CreateNotificationChannel(chan);

                Intent notificationIntent = new Intent(this, typeof(DatabaseInsertions));
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);

                var notification = new Notification.Builder(this, NOTIFICATION_CHANNEL_ID)
                .SetContentTitle("FIU Assist")
                .SetContentText("Storing/Pushing Data")
                .SetSmallIcon(Resource.Drawable.health_icon_p)
                .SetContentIntent(pendingIntent)
                .SetOngoing(true)
                .Build();


                CollectionService();


                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }

            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }


        public static bool isRunning;
        private void CollectionService()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!isRunning)
                    {
                        var sen = new SensorCollectionManager();
                        //sen.PhoneSensorOperationsAsync(_cts.Token);
                    }
                    
                }
                catch (Exception ex)
                {
                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new CancelledMessage();
                        Device.BeginInvokeOnMainThread(
                            () => MessagingCenter.Send(message, "CancelledMessage")
                        );
                    }                  
                }
            }, _cts.Token);
        }

        public override void OnDestroy()
        {
            //Log.Debug(logTag, "Service has been terminated");
            //StopSelf();
            var _message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(_message, "StartLongRunningCollection");
            //SensorCollectionManager.destroy_service = true;
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }

            base.OnDestroy();
        }
    }
}