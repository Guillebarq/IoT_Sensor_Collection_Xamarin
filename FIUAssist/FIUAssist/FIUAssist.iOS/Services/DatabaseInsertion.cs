using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using FIUAssist.Services;
using FIUAssist.Messages;
using UIKit;
using Xamarin.Forms;
using FIUAssist.Utils;

namespace FIUAssist.iOS.Services
{
    class DatabaseInsertion
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
                var net = new NetworkCommunicationManager();
                //await net.DBOperations(_cts.Token);
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
                else
                {

                    StaticObjects.IsCollecting = false;

                    var message = new CollectionMessage
                    {
                        Message = "Push Complete"
                    };
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "CollectionMessage")
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