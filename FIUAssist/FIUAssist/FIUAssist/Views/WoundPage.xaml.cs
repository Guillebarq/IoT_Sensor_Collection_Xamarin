using FIUAssist.Utils;
using FIUAssist.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FIUAssist.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WoundPage : ContentPage
	{

        public static Action<double[]> SetWoundData;
        public static Action<bool> ConnectionDelete;

        private ObservableCollection<LmpoutputViewModel> woundLmpoutputValues1;
        private ObservableCollection<LmpoutputViewModel> woundLmpoutputValues2;
        private ObservableCollection<LmpoutputViewModel> woundLmpoutputValues3;
        private ObservableCollection<LmpoutputViewModel> woundLmpoutputValues4;

        private float temperature;
        private DateTime currentTimeWound;

        public static bool Connected;

        public WoundPage ()
		{
			InitializeComponent ();

            WoundLmpoutputValues1 = new ObservableCollection<LmpoutputViewModel>();
            WoundLmpoutputValues2 = new ObservableCollection<LmpoutputViewModel>();
            WoundLmpoutputValues3 = new ObservableCollection<LmpoutputViewModel>();
            WoundLmpoutputValues4 = new ObservableCollection<LmpoutputViewModel>();

            SetWoundData = UpdateWoundSensorData;
            ConnectionDelete = ClearLists;

            BindingContext = this;

        }

        public ObservableCollection<LmpoutputViewModel> WoundLmpoutputValues1
        {
            get
            {
                return woundLmpoutputValues1;
            }

            set
            {
                this.woundLmpoutputValues1 = value;
                OnPropertyChanged("WoundLmpoutputValues1");
            }
        }

        public ObservableCollection<LmpoutputViewModel> WoundLmpoutputValues2
        {
            get
            {
                return woundLmpoutputValues2;
            }

            set
            {
                this.woundLmpoutputValues2 = value;
                OnPropertyChanged("WoundLmpoutputValues2");
            }
        }

        public ObservableCollection<LmpoutputViewModel> WoundLmpoutputValues3
        {
            get
            {
                return woundLmpoutputValues3;
            }

            set
            {
                this.woundLmpoutputValues3 = value;
                OnPropertyChanged("WoundLmpoutputValues3");
            }
        }

        public ObservableCollection<LmpoutputViewModel> WoundLmpoutputValues4
        {
            get
            {
                return woundLmpoutputValues4;
            }

            set
            {
                this.woundLmpoutputValues4 = value;
                OnPropertyChanged("WoundLmpoutputValues4");
            }
        }

        public float Temperature
        {
            get
            {
                return temperature;
            }

            set
            {
                this.temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        private int channel;
        public int Channel
        {
            get { return channel; }
            set
            {
                this.channel = value;
                OnPropertyChanged("Channel");
            }
        }

        private int batteryVoltage;

        public int BatteryVoltage
        {
            get
            {
                return batteryVoltage;
            }
            set
            {
                batteryVoltage = value;
                OnPropertyChanged("BatteryVoltage");
            }
        }

        private double lmpout;

        public double Lmpout
        {
            get
            {
                return lmpout;
            }
            set
            {
                lmpout = value;
                OnPropertyChanged("Lmpout");
            }
        }

        private double bias;

        public double Bias
        {
            get { return bias; }
            set {
                bias = value;
                OnPropertyChanged("Bias");
            }
        }


        private bool startDeleteWound1 = false;
        private bool startDeleteWound2 = false;
        private bool startDeleteWound3 = false;
        private bool startDeleteWound4 = false;

        private int countWound1;
        private int countWound2;
        private int countWound3;
        private int countWound4;

        double diff1, diff2, diff3, diff4;

        public void ClearLists(bool connected)
        {
            if (connected == false)
            {
                woundLmpoutputValues1.Clear();
                woundLmpoutputValues2.Clear();
                woundLmpoutputValues3.Clear();
                woundLmpoutputValues4.Clear();
                Lmpout = 0;
                Temperature = 0;
                BatteryVoltage = 0;
                Channel = 0;
                Bias = 0;
                countWound1 = 0;
                countWound2 = 0;
                countWound3 = 0;
                countWound4 = 0;
                startDeleteWound1 = false;
                startDeleteWound2 = false;
                startDeleteWound3 = false;
                startDeleteWound4 = false;
            }
            
        }

        void UpdateWoundSensorData(double[] woundSensors)
        {
            Device.BeginInvokeOnMainThread(() => {
                //double diff = ((DateTime.Now - currentTimeWound).TotalMilliseconds) / 1000.0;

                Channel = (int)woundSensors[Constants.CHANNEL];

                Temperature = (float)woundSensors[Constants.TEMPERATURE];

                BatteryVoltage = (int)woundSensors[Constants.BATTERY_VOLTAGE];

                Lmpout = woundSensors[Constants.LMPOUTPUT];

                Bias = woundSensors[Constants.BIAS];

                switch (woundSensors[Constants.CHANNEL])
                {
                    case 1:

                        diff1 = countWound1++;
                        this.woundLmpoutputValues1.Add(new LmpoutputViewModel
                        {
                            LmpoutputTime = diff1,
                            Lmpoutput = woundSensors[Constants.LMPOUTPUT]
                        });

                        if (woundLmpoutputValues2.Count != 0)
                        {
                            woundLmpoutputValues2.Clear();
                            countWound2 = 0;
                            diff2 = 0;
                            startDeleteWound2 = false;
                        }
                        else if (woundLmpoutputValues3.Count != 0)
                        {
                            woundLmpoutputValues3.Clear();
                            countWound3 = 0;
                            diff3 = 0;
                            startDeleteWound3 = false;
                        }
                        else if (woundLmpoutputValues4.Count != 0)
                        {
                            woundLmpoutputValues4.Clear();
                            countWound4 = 0;
                            diff4 = 0;
                            startDeleteWound4 = false;
                        }

                        if (diff1 > 20)
                        {

                            if (countWound1 > 2)
                            {
                                startDeleteWound1 = true;
                            }

                            if (startDeleteWound1 == true && woundSensors[Constants.CHANNEL] == 1)
                            {
                                woundLmpoutputValues1.RemoveAt(0);

                            }

                        }
                        break;
                    case 2:

                        diff2 = countWound2++;
                        this.woundLmpoutputValues2.Add(new LmpoutputViewModel
                        {
                            LmpoutputTime = diff2,
                            Lmpoutput = woundSensors[Constants.LMPOUTPUT]
                        });

                        if (woundLmpoutputValues1.Count != 0)
                        {
                            woundLmpoutputValues1.Clear();
                            countWound1 = 0;
                            diff1 = 0;
                            startDeleteWound1 = false;
                        }
                        else if (woundLmpoutputValues3.Count != 0)
                        {
                            woundLmpoutputValues3.Clear();
                            countWound3 = 0;
                            diff3 = 0;
                            startDeleteWound3 = false;
                        }
                        else if (woundLmpoutputValues4.Count != 0)
                        {
                            woundLmpoutputValues4.Clear();
                            countWound4 = 0;
                            diff4 = 0;
                            startDeleteWound4 = false;
                        }

                        if (diff2 > 20)
                        {

                            if (countWound2 > 2)
                            {
                                startDeleteWound2 = true;
                            }

                            if (startDeleteWound2 == true && woundSensors[Constants.CHANNEL] == 2)
                            {

                                woundLmpoutputValues2.RemoveAt(0);
                            }

                        }

                        break;
                    case 3:

                        diff3 = countWound3++;
                        this.woundLmpoutputValues3.Add(new LmpoutputViewModel
                        {
                            LmpoutputTime = diff3,
                            Lmpoutput = woundSensors[Constants.LMPOUTPUT]
                        });

                        if (woundLmpoutputValues1.Count != 0)
                        {
                            woundLmpoutputValues1.Clear();
                            countWound1 = 0;
                            diff1 = 0;
                            startDeleteWound1 = false;
                        }
                        else if (woundLmpoutputValues2.Count != 0)
                        {
                            woundLmpoutputValues2.Clear();
                            countWound2 = 0;
                            diff2 = 0;
                            startDeleteWound2 = false;
                        }
                        else if (woundLmpoutputValues4.Count != 0)
                        {
                            woundLmpoutputValues4.Clear();
                            countWound4 = 0;
                            diff4 = 0;
                            startDeleteWound4 = false;
                        }

                        if (diff3 > 20)
                        {

                            if (countWound3 > 2)
                            {
                                startDeleteWound3 = true;
                            }

                            if (startDeleteWound3 == true && woundSensors[Constants.CHANNEL] == 3)
                            {
                                woundLmpoutputValues3.RemoveAt(0);
                            }

                        }

                        break;
                    case 4:

                        diff4 = countWound4++;
                        this.woundLmpoutputValues4.Add(new LmpoutputViewModel
                        {
                            LmpoutputTime = diff4,
                            Lmpoutput = woundSensors[Constants.LMPOUTPUT]
                        });

                        if (woundLmpoutputValues1.Count != 0)
                        {
                            woundLmpoutputValues1.Clear();
                            countWound1 = 0;
                            diff1 = 0;
                            startDeleteWound1 = false;
                        }
                        else if (woundLmpoutputValues3.Count != 0)
                        {
                            woundLmpoutputValues3.Clear();
                            countWound3 = 0;
                            diff3 = 0;
                            startDeleteWound3 = false;
                        }
                        else if (woundLmpoutputValues2.Count != 0)
                        {
                            woundLmpoutputValues2.Clear();
                            countWound2 = 0;
                            diff2 = 0;
                            startDeleteWound2 = false;
                        }

                        if (diff4 > 20)
                        {

                            if (countWound4 > 2)
                            {
                                startDeleteWound4 = true;
                            }

                            if (startDeleteWound4 == true && woundSensors[Constants.CHANNEL] == 4)
                            {
                                woundLmpoutputValues4.RemoveAt(0);
                            }

                        }

                        break;

                }

                if (countWound4 > 500 && countWound3 > 500 && countWound2 > 500 && countWound1 > 500)
                {
                    woundLmpoutputValues1.Clear();
                    woundLmpoutputValues2.Clear();
                    woundLmpoutputValues3.Clear();
                    woundLmpoutputValues4.Clear();
                    countWound1 = 0;
                    countWound2 = 0;
                    countWound3 = 0;
                    countWound4 = 0;
                    startDeleteWound1 = false;
                    startDeleteWound2 = false;
                    startDeleteWound3 = false;
                    startDeleteWound4 = false;
                }

            });
        }


    }
}