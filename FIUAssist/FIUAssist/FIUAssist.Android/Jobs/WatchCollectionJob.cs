using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App.Job;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FIUAssist.Utils;
using System.Threading;
using FIUAssist.Bluetooth;
using FIUAssist.Views;
using FIUAssist.Messages;
using Xamarin.Forms;

namespace FIUAssist.Droid.Jobs
{
    [Service(Name = "com.companyname.FIUAssist.Jobs.WatchCollectionJob", Permission = "android.permission.BIND_JOB_SERVICE")]
    class WatchCollectionJob : JobService
    {

        Thread WatchCollectionThread;

        public override bool OnStartJob(JobParameters @params)
        {
            try
            {
                var w = new BluetoothWatchConnection();
                WatchCollectionThread = new Thread(w.ConnectToWatchAsync);
                WatchCollectionThread.Start();
            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }

            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {

            WatchCollectionThread.Abort();

            WatchCollectionThread = null;

            if (SmartWatchPage.ClearGraphs != null)
            {
                SmartWatchPage.ClearGraphs();
            }

            var message = new CollectionMessage{ Message = "Connection Restarting..." };
            Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, "CollectionMessage"));

            return true;
        }
    }
}