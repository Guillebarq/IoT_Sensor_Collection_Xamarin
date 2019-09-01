using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FIUAssist.ViewModels
{
    class LogViewModel: INotifyPropertyChanged
    {
        private string _log;

        public string _Log
        {
            get { return _log; }
            set
            {
                _log = value;
                OnPropertyChanged("_Log");
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
