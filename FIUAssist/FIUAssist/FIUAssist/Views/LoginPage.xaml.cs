using FIUAssist.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace FIUAssist.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
            var vm = new LoginViewModel();
            this.BindingContext = vm;
            vm.DisplayInvalidLoginPrompt += () => DisplayAlert("Error", "Invalid Login, try again", "OK");
            InitializeComponent();

            Email.Completed += (object sender, EventArgs e) =>
            {
                Password.Focus();
            };

            Password.Completed += async (object sender, EventArgs e) =>
            {
                vm.SubmitCommand.Execute(null);
                if (App.IsUserLoggedIn == true)
                {
                    //Xamarin.Forms.TabbedPage tabbedPage = new Xamarin.Forms.TabbedPage();
                    //tabbedPage.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
                    //NavigationPage navigationPage = new NavigationPage(new MainPage());
                    //MainPage m = new MainPage();
                    //m.IconImageSource = "phone_icon.ico";
                    //m.Title = "Phone";
                    //tabbedPage.Children.Add(m);
                    //tabbedPage.Children.Add(new WoundPage());
                    //tabbedPage.Children.Add(new BluetoothList());
                    //Navigation.InsertPageBefore(tabbedPage, this);
                    //await Navigation.PopAsync();
                }               
            };

            
		}
	}
}