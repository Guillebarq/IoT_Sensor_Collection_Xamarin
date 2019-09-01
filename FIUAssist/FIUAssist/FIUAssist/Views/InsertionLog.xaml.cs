using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FIUAssist.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InsertionLog : ContentPage
    {
        ObservableCollection<string> datalog;
        public static Action<string> insertLog;

        public InsertionLog()
        {
            InitializeComponent();

            insertLog = LogData;

            datalog = new ObservableCollection<string>();

            LogList.ItemsSource = datalog;    

        }

        private void LogData(string log)
        {
            if (datalog.Count > 0)
            {
                datalog.RemoveAt(0);
            }
            datalog.Add(log);
        }
    }
}