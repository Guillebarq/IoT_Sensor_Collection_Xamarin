using FIUAssist.Utils;
using FIUAssist.ViewModels;
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
	public partial class SmartWatchPage : ContentPage
	{
        public static Action<double[]> SetWatchData;
        public static Action ClearGraphs;

        private ObservableCollection<AccelerationViewModel> watchAccelerationValues;
        private ObservableCollection<MagnetometerViewModel> watchMagnetometerValues;
        private ObservableCollection<GyroscopeViewModel> watchGyroscopeValues;
        private ObservableCollection<HeartRateViewModel> heartRateValues;
        //private ObservableCollection<StepCountChartModel> stepCountValues;

        private int countWatch;
        private bool startDeleteWatch = false;


        public SmartWatchPage ()
		{
			InitializeComponent ();

            SetWatchData = UpdateWatchData;
            ClearGraphs = ClearDisplay;

            WatchAccelerationValues = new ObservableCollection<AccelerationViewModel>();
            WatchMagnetometerValues = new ObservableCollection<MagnetometerViewModel>();
            WatchGyroscopeValues = new ObservableCollection<GyroscopeViewModel>();
            HeartRateValues = new ObservableCollection<HeartRateViewModel>();

            this.BindingContext = this;

        }


        void UpdateWatchData(double[] values)
        {
            //update on main thread but collect data in background thread
            Device.BeginInvokeOnMainThread(() => {

                double diff = countWatch++;

                //this.StepCount = (int)values[Constants.StepCount];


                this.watchAccelerationValues.Add(new AccelerationViewModel
                {
                    AccelerationTime = diff,
                    XAccelerationValue = values[Constants.XAccelerationValue],
                    YAccelerationValue = values[Constants.YAccelerationValue],
                    ZAccelerationValue = values[Constants.ZAccelerationValue]
                });

                this.watchMagnetometerValues.Add(new MagnetometerViewModel
                {
                    MagnetometerTime = diff,
                    XMagnetometerValue = values[Constants.XMagnetometerValue],
                    YMagnetometerValue = values[Constants.YMagnetometerValue],
                    ZMagnetometerValue = values[Constants.ZMagnetometerValue]
                });

                this.watchGyroscopeValues.Add(new GyroscopeViewModel
                {
                    GyroscopeTime = diff,
                    XGyroscopeValue = values[Constants.XGyroscopeValue],
                    YGyroscopeValue = values[Constants.YGyroscopeValue],
                    ZGyroscopeValue = values[Constants.ZGyroscopeValue]
                });

                this.heartRateValues.Add(new HeartRateViewModel
                {

                    HeartRateTime = diff,
                    HeartRate = (int)values[Constants.HeartRate]

                });

                //this.HeartRateNum = (int)values[Constants.HeartRate];

                try
                {
                    if (startDeleteWatch == true)
                    {
                        watchAccelerationValues.RemoveAt(0);
                        watchMagnetometerValues.RemoveAt(0);
                        watchGyroscopeValues.RemoveAt(0);
                        heartRateValues.RemoveAt(0);
                    }
                    if (diff > 20)
                    {
                        if (countWatch > 2)
                        {
                            startDeleteWatch = true;
                        }

                        if (countWatch > 500)
                        {
                            watchAccelerationValues.Clear();
                            watchMagnetometerValues.Clear();
                            watchGyroscopeValues.Clear();
                            heartRateValues.Clear();
                            countWatch = 0;
                            startDeleteWatch = false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    //ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                }
            });
        }

        public void ClearDisplay()
        {
            heartRateValues.Clear();
            watchAccelerationValues.Clear();
            watchMagnetometerValues.Clear();
            watchGyroscopeValues.Clear();
            countWatch = 0;
            startDeleteWatch = false;
        }


        public ObservableCollection<AccelerationViewModel> WatchAccelerationValues
        {
            get
            {
                return watchAccelerationValues;
            }

            set
            {
                this.watchAccelerationValues = value;
                OnPropertyChanged("WatchAccelerationValues");
            }
        }


        public ObservableCollection<MagnetometerViewModel> WatchMagnetometerValues
        {
            get
            {
                return watchMagnetometerValues;
            }

            set
            {
                this.watchMagnetometerValues = value;
                OnPropertyChanged("WatchMagnetometerValues");
            }
        }


        public ObservableCollection<GyroscopeViewModel> WatchGyroscopeValues
        {
            get
            {
                return watchGyroscopeValues;
            }

            set
            {
                this.watchGyroscopeValues = value;
                OnPropertyChanged("WatchGyroscopeValues");
            }
        }

        public ObservableCollection<HeartRateViewModel> HeartRateValues
        {
            get
            {
                return heartRateValues;
            }

            set
            {
                this.heartRateValues = value;
                OnPropertyChanged("HeartRateValues");
            }
        }

    }
}