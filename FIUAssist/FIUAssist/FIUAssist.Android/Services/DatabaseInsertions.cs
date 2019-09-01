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
using FIUAssist.Views;
using Xamarin.Forms;

namespace FIUAssist.Droid.Services
{
    [Service]
    class DatabaseInsertions : Service
    {

        CancellationTokenSource _cts;

        //public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        IBinder binder;
        readonly string logTag = "CollectionService";

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(logTag, "Client now bound to service");

            binder = new CollectionServiceBinder(this);
            return binder;
        }



        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            //if(intent.Action.Equals())


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


                StoreService();
                

                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
                
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            //StartForeground(1, new Notification());
        }
        public static bool isRunning;
        public void StoreService()
        {
            Task.Run(() =>
            {
                try
                {
                    //INVOKE THE SHARED CODE
                    if (!isRunning)
                    {
                        var net = new NetworkCommunicationManager();
                        //net.DBOperations(_cts.Token);
                    }
                    
                }
                catch (Exception ex)
                {

                    ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());

                    var message = new CollectionMessage
                    {
                        Message = "Connection Error"
                    };
                    Device.BeginInvokeOnMainThread(
                            () => MessagingCenter.Send(message, "CollectionMessage")
                    );
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
                    else
                    {
                        StaticObjects.IsCollecting = false;
                        //var _message = new StopLongRunningTaskMessage();
                        //MessagingCenter.Send(_message, "StopLongRunningTaskMessage");

                        var message = new CollectionMessage
                        {
                            Message = "Database push resuming..."
                        };
                        Device.BeginInvokeOnMainThread(
                            () => MessagingCenter.Send(message, "CollectionMessage")
                        );
                    }
                }
            }, _cts.Token);
        }

    //    public void onTaskRemoved(Intent rootIntent)
    //    {
    //        base.OnTaskRemoved(rootIntent);
    //        PendingIntent service = PendingIntent.GetService(
    //                ApplicationContext,
    //                1001,
    //                new Intent(ApplicationContext, typeof(DatabaseInsertions)),
    //        Android.App.PendingIntent.FLAG_ONE_SHOT);

    //    AlarmManager alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);
    //    alarmManager.Set(AlarmManager.ELAPSED_REALTIME_WAKEUP, 1000, service);
    //}

        public override void OnDestroy()
        {
            //Log.Debug(logTag, "Service has been terminated");
            //StopSelf();
            var _message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(_message, "StartLongRunningTaskMessage");
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }

            base.OnDestroy();
        }

    }
}