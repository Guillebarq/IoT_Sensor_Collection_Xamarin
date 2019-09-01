using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FIUAssist.Messages;
using FIUAssist.Services;
using FIUAssist.Utils;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace FIUAssist.iOS.Services
{
    class SensorCollectionService
    {
        nint _taskId;
        CancellationTokenSource _cts;
        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            StaticObjects.IsCollecting = true;
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("LongRunningTask", OnExpiration);

            try
            {
                var net = new SensorCollectionManager();
                //await net.PhoneSensorOperationsAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {

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

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);

        }

        public void Stop()
        {
            _cts.Cancel();
        }

        void OnExpiration()
        {
            _cts.Cancel();
        }
    }
}