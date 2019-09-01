using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FIUAssist.ViewModels
{
    public class GyroscopeViewModel : INotifyPropertyChanged
    {
        private double gyroscopeTime, xGyroscopeValue, yGyroscopeValue, zGyroscopeValue;

        public double GyroscopeTime
        {

            get { return gyroscopeTime; }
            set
            {
                gyroscopeTime = value;
                OnPropertyChanged("GyroscopeTime");
            }
        }

        public double XGyroscopeValue
        {
            get { return xGyroscopeValue; }
            set
            {
                xGyroscopeValue = value;
                OnPropertyChanged("XGyroscopeValue");
            }
        }

        public double YGyroscopeValue
        {
            get { return yGyroscopeValue; }
            set
            {
                yGyroscopeValue = value;
                OnPropertyChanged("YGyroscopeValue");
            }
        }

        public double ZGyroscopeValue
        {
            get { return zGyroscopeValue; }
            set
            {
                zGyroscopeValue = value;
                OnPropertyChanged("ZGyroscopeValue");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
