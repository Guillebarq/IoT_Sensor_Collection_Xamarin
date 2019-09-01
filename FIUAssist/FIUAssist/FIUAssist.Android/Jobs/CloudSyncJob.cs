using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using FIUAssist.DatabaseManager;
using FIUAssist.Messages;
using FIUAssist.Services;
using FIUAssist.Utils;
using FIUAssist.Views;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace FIUAssist.Droid.Jobs
{
    [Service(Name = "com.companyname.FIUAssist.Jobs.CloudSyncJob", Permission = "android.permission.BIND_JOB_SERVICE")]
    class CloudSyncJob : JobService
    {

        Thread PhoneThread;
        Thread WoundThread;
        Thread WatchThread;
        Thread CloudSyncThread;

        public static bool isRunning;
        CancellationTokenSource _cts;
        public override bool OnStartJob(JobParameters jobParameters)
        {

            Task.Run(async () => 
            {
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

                    var _message = new CollectionMessage { Message = "Error Pushing" };
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(_message, "CollectionMessage"));

                    var message = new NotifyMessage { Message = "Error pushing to cloud. The job will restart soon (Maximum 15 minutes)" };
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(_message, "NotifyMessage"));
                }
            });


            //Task.Run(async () =>
            //{

            //    try
            //    {
            //        if (!isRunning)
            //        {
            //            var sen = new SensorCollectionManager();
            //            await sen.PhoneSensorOperations(_cts.Token);
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            //    }
            //    finally
            //    {
            //        if (_cts.IsCancellationRequested)
            //        {
            //            var message = new CancelledMessage();
            //            Device.BeginInvokeOnMainThread(
            //                () => MessagingCenter.Send(message, "CancelledMessage")
            //            );
            //        }
            //    }

            //    JobFinished(jobParameters, false);
            //});

            return true;
        }


        public override bool OnStopJob(JobParameters jobParams)
        {

            //StaticObjects.jobCancelled = true;

            //PhoneThread.Abort();
            //WoundThread.Abort();
            //WatchThread.Abort();
            //CloudSyncThread.Abort();

            //PhoneThread = null;
            // WoundThread = null;
            //WatchThread = null;
            //CloudSyncThread = null;

            //if (WoundPage.ConnectionDelete != null)
            //{
            //    WoundPage.ConnectionDelete(true);
            //}

            var message = new CollectionMessage
            {
                Message = "Job Restarting..."
            };
            Device.BeginInvokeOnMainThread(
                () => MessagingCenter.Send(message, "CollectionMessage")
            );

            return true;
        }

    }
}