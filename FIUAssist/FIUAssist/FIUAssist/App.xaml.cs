using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FIUAssist.Views;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using FIUAssist.Messages;
using FIUAssist.Utils;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FIUAssist
{
    public partial class App : Xamarin.Forms.Application
    {
        public static bool IsUserLoggedIn { get; set; }
        public App()
        {
            InitializeComponent();

            Xamarin.Forms.TabbedPage tabbedPage = new Xamarin.Forms.TabbedPage();
            tabbedPage.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
           // On<Xamarin.Forms.PlatformConfiguration.Android>).SetToolbarPlacement(ToolbarPlacement.Bottom);
            //var mainPage = new MainPage();
            NavigationPage navigationPage = new NavigationPage(new MainPage());
            navigationPage.IconImageSource = "phone_icon.ico";
            navigationPage.Title = "Phone";
            tabbedPage.Children.Add(navigationPage);
            tabbedPage.Children.Add(new WoundPage());
            tabbedPage.Children.Add(new SmartWatchPage());
            tabbedPage.Children.Add(new BluetoothList());
            MainPage = tabbedPage;
        }

        void CheckState()
        {
            //Xamarin.Forms.Application.Current.Properties["push_state"] = 0;
        }

        protected override void OnStart()
        {
            //var message = new StartLongRunningCollection();
            //MessagingCenter.Send(message, "StartLongRunningCollection");

            //var _message = new StartLongRunningTaskMessage();
            //MessagingCenter.Send(_message, "StartLongRunningTaskMessage");

        }

        protected override void OnSleep()
        {
     
            
        }

        protected override void OnResume()
        {

        }
    }
}
