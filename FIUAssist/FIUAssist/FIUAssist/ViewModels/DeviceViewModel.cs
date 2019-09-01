using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FIUAssist.ViewModels
{
    class DeviceViewModel : INotifyPropertyChanged
    {
        private IDevice _nativeDevice;

        public event PropertyChangedEventHandler PropertyChanged;

        public IDevice NativeDevice
        {
            get
            {
                return _nativeDevice;
            }
            set
            {
                _nativeDevice = value;
                OnPropertyChanged();
            }

        }

        public void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }     
        }

    }
}
