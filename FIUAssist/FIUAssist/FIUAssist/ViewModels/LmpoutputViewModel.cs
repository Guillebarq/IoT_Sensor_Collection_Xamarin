using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FIUAssist.ViewModels
{
    public class LmpoutputViewModel: INotifyPropertyChanged
    {
        double lmpoutput, lmpoutputTime;

        public double LmpoutputTime
        {

            get { return lmpoutputTime; }
            set
            {
                lmpoutputTime = value;
                OnPropertyChanged("LmpoutputTime");
            }
        }

        public double Lmpoutput
        {

            get { return lmpoutput; }
            set
            {
                lmpoutput = value;
                OnPropertyChanged("Lmpoutput");
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
