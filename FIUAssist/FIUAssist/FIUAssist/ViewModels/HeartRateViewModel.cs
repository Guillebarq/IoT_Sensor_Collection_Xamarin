using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FIUAssist.ViewModels
{
    public class HeartRateViewModel : INotifyPropertyChanged
    {
        private int heartRate;
        private double heartRateTime;

        public double HeartRateTime
        {

            get { return heartRateTime; }
            set
            {
                heartRateTime = value;
                OnPropertyChanged("HeartRateTime");
            }
        }

        public int HeartRate
        {

            get { return heartRate; }
            set
            {
                heartRate = value;
                OnPropertyChanged("HeartRate");
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
