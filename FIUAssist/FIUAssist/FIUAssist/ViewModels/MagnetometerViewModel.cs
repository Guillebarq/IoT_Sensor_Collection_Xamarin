using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FIUAssist.ViewModels
{
    public class MagnetometerViewModel: INotifyPropertyChanged
    {
        private double magnetometerTime, xMagnetometerValue, yMagnetometerValue, zMagnetometerValue;

        public double MagnetometerTime
        {

            get { return magnetometerTime; }
            set
            {
                magnetometerTime = value;
                OnPropertyChanged("MagnetometerTime");
            }
        }

        public double XMagnetometerValue
        {
            get { return xMagnetometerValue; }
            set
            {
                xMagnetometerValue = value;
                OnPropertyChanged("XMagnetometerValue");
            }
        }

        public double YMagnetometerValue
        {
            get { return yMagnetometerValue; }
            set
            {
                yMagnetometerValue = value;
                OnPropertyChanged("YMagnetometerValue");
            }
        }

        public double ZMagnetometerValue
        {
            get { return zMagnetometerValue; }
            set
            {
                zMagnetometerValue = value;
                OnPropertyChanged("ZMagnetometerValue");
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
