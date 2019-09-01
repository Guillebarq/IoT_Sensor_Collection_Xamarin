using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using FIUAssist.Messages;
using Android.Content;
using FIUAssist.Droid.Services;
using Java.Lang;
using FIUAssist.Utils;
using System.Threading.Tasks;
using Android.App.Job;
using FIUAssist.Droid.Jobs;

namespace FIUAssist.Droid
{
    [Activity(Label = "FIUAssist", Icon = "@mipmap/health_icon_p", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        JobScheduler jobScheduler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            //WireUpLongRunningTask();
            jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);


            AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidEnvironmentUnhandledExceptionRaiser;
            TaskScheduler.UnobservedTaskException += OnAndroidEnvironmentUnhandledThreadExceptionRaiser;

            var javaClass = Java.Lang.Class.FromType(typeof(CloudSyncJob));
            var compName = new ComponentName(this, javaClass);
            var jobInfo = new JobInfo.Builder(jobId: 27, jobService: compName)
                .SetPeriodic(900000)
                .SetRequiredNetworkType(NetworkType.Any)
                .SetPersisted(true)
                .Build();

             var javaPhoneClass = Java.Lang.Class.FromType(typeof(PhoneCollectionJob));
            var compPhoneName = new ComponentName(this, javaPhoneClass);
            var jobInfoPhone = new JobInfo.Builder(jobId: 29, jobService: compPhoneName)
                .SetPeriodic(900000)
                .SetPersisted(true)
                .Build();

            var javaWatchClass = Java.Lang.Class.FromType(typeof(WatchCollectionJob));
            var compWatchName = new ComponentName(this, javaWatchClass);
            var jobInfoWatch = new JobInfo.Builder(jobId: 28, jobService: compWatchName)
                .SetPeriodic(900000)
                .SetPersisted(true)
                .Build();
           

            CheckJobIntegrity(jobInfo, jobInfoWatch, jobInfoPhone);
            
        }

        private void CheckJobIntegrity(JobInfo jobInfo, JobInfo jobInfoWatch, JobInfo jobInfoPhone)
        {
            var result = jobScheduler.Schedule(jobInfo);

            if (result != JobScheduler.ResultSuccess)
            {

                ExceptionErrorLogger.writeFileOnInternalStorage("THE JOB HAS FAILED DUE TO NO WIFI CONNECTION. NOTHING WAS COLLECTED AND NOTHING WAS PUSHED AFTER THIS STATEMENT");

                var _message = new NotifyMessage{ Message = "No internet connection. Please restart the app under a stable connection."};
                Device.BeginInvokeOnMainThread(() =>{ MessagingCenter.Send(_message, "NotifyMessage");});
            }

            var result_watch = jobScheduler.Schedule(jobInfoWatch);

            if (result_watch != JobScheduler.ResultSuccess)
            {

                ExceptionErrorLogger.writeFileOnInternalStorage("THE WATCH JOB DID NOT START. RESTART APP.");

                var _message = new ErrorMessage { Message = "Job Failure" };
                Device.BeginInvokeOnMainThread(() => { MessagingCenter.Send(_message, "ErrorMessage"); });
            }

            var result_phone = jobScheduler.Schedule(jobInfoPhone);

            if (result_phone != JobScheduler.ResultSuccess)
            {

                ExceptionErrorLogger.writeFileOnInternalStorage("THE PHONE JOB DID NOT START. RESTART APP.");

                var _message = new ErrorMessage { Message = "Job Failure" };
                Device.BeginInvokeOnMainThread(() => { MessagingCenter.Send(_message, "ErrorMessage"); });
            }


        }
        

        private void OnAndroidEnvironmentUnhandledThreadExceptionRaiser(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new System.Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            ExceptionErrorLogger.writeFileOnInternalStorage(newExc.ToString());

            Console.WriteLine(newExc);

        }

        private void OnAndroidEnvironmentUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new System.Exception("OnAndroidEnvironmentUnhandledExceptionRaiser", unhandledExceptionEventArgs.Exception);

            ExceptionErrorLogger.writeFileOnInternalStorage(newExc.ToString());

            Console.WriteLine(newExc);

        }
        private bool isMyServiceRunning(System.Type cls)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);

            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }

        public static Intent intent;
        public static Intent intent_col;

        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", message => {

                var javaClass = Java.Lang.Class.FromType(typeof(CloudSyncJob));
                var compName = new ComponentName(this, javaClass);
                var jobInfo = new JobInfo.Builder(jobId: 27, jobService: compName)
                    .SetMinimumLatency(30000)
                    .SetOverrideDeadline(60000)
                    .SetPeriodic(900000)
                    .Build();

                var result = jobScheduler.Schedule(jobInfo);
                if (result != JobScheduler.ResultSuccess)
                {
                    var _message = new ErrorMessage
                    {
                        Message = "Job Failure"
                    };
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send(_message, "ErrorMessage");
                    });
                }

                //intent = new Intent(this, typeof(DatabaseInsertions));
                //StartForegroundService(intent);
                //intent_col = new Intent(this, typeof(SensorCollectionService));
                //StartForegroundService(intent_col);
            });

            //MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
            //    intent = new Intent(this, typeof(DatabaseInsertions));
            //    if (intent != null)
            //    {
            //        StopService(intent);
            //    } 
            //});

            MessagingCenter.Subscribe<StartLongRunningCollection>(this, "StartLongRunningCollection", message => {
                Device.BeginInvokeOnMainThread(() => {
                    intent_col = new Intent(this, typeof(SensorCollectionService));
                    StartForegroundService(intent_col);
                });
            });

            //MessagingCenter.Subscribe<StopLongRunningCollection>(this, "StopLongRunningCollection", message =>
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        var intent_col = new Intent(this, typeof(SensorCollectionService));
            //        StopService(intent_col);
            //    });
            //});

        }

    }

    public static class JobSchedulerHelpers
    {
        public static JobInfo.Builder CreateJobBuilderUsingJobId<CloudSyncJob>(this Context context, int jobId) where CloudSyncJob : JobService
        {
            var javaClass = Java.Lang.Class.FromType(typeof(CloudSyncJob));
            var componentName = new ComponentName(context, javaClass);
            return new JobInfo.Builder(jobId, componentName);
        }
    }

}