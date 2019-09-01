using System;
using System.Collections.Generic;
using System.Linq;
using FIUAssist.iOS.Services;
using FIUAssist.Messages;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace FIUAssist.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        DatabaseInsertion longDbTask;
        SensorCollectionService collectDataTask;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Syncfusion.SfChart.XForms.iOS.Renderers.SfChartRenderer.Init();
            Syncfusion.SfDataGrid.XForms.iOS.SfDataGridRenderer.Init();
            LoadApplication(new App());
            WireUpLongRunningTask();
            return base.FinishedLaunching(app, options);
        }


        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", async message => {
                longDbTask = new DatabaseInsertion();
                await longDbTask.Start();
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
                longDbTask.Stop();
            });

            MessagingCenter.Subscribe<StartLongRunningCollection>(this, "StartLongRunningCollection", async message => {
                collectDataTask = new SensorCollectionService();
                await collectDataTask.Start();
            });

            MessagingCenter.Subscribe<StopLongRunningCollection>(this, "StopLongRunningCollection", message => {
                collectDataTask.Stop();
            });

        }

    }
}
