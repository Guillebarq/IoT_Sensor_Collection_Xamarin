using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FIUAssist.ViewModels
{
    public class AccelerationViewModel: INotifyPropertyChanged
    {
        private double accelerationTime, xAccelerationValue, yAccelerationValue, zAccelerationValue;

        public double AccelerationTime
        {

            get { return accelerationTime; }
            set
            {
                accelerationTime = value;
                OnPropertyChanged("AccelerationTime");
            }
        }
        public double XAccelerationValue
        {

            get { return xAccelerationValue; }
            set
            {
                xAccelerationValue = value;
                OnPropertyChanged("XAccelerationValue");
            }
        }


        public double YAccelerationValue
        {

            get { return yAccelerationValue; }
            set
            {
                yAccelerationValue = value;
                OnPropertyChanged("YAccelerationValue");
            }
        }


        public double ZAccelerationValue
        {

            get { return zAccelerationValue; }
            set
            {
                zAccelerationValue = value;
                OnPropertyChanged("ZAccelerationValue");
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
