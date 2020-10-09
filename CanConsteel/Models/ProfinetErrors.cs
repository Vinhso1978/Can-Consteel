using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    class Errors: INotifyPropertyChanged
    {
        public bool REQUpdate;
        private bool _act350;
        public bool ACT350
        {
            get { return _act350; }
            set
            {
                bool oldValue = _act350;
                _act350 = value;
                if (oldValue != value)
                {
                    OnPropertyChanged("ACT350");
                }

            }
        }

        private bool _openLeft;
        public bool OpenLeft
        {
            get { return _openLeft; }
            set
            {
                bool oldValue = _openLeft;
                _openLeft = value;
                if (oldValue != value)
                {
                    OnPropertyChanged("OpenLeft");
                }

            }
        }

        private bool _openRight;
        public bool OpenRight
        {
            get { return _openRight; }
            set
            {
                bool oldValue = _openRight;
                _openRight = value;
                if (oldValue != value)
                {
                    OnPropertyChanged("OpenRight");
                }

            }
        }

        private bool _closeLeft;
        public bool CloseLeft
        {
            get { return _closeLeft; }
            set
            {
                bool oldValue = _closeLeft;
                _closeLeft = value;
                if (oldValue != value)
                {
                    OnPropertyChanged("CloseLeft");
                }

            }
        }

        private bool _closeRight;
        public bool CloseRight
        {
            get { return _closeRight; }
            set
            {
                bool oldValue = _closeRight;
                _closeRight = value;
                if (oldValue != value)
                {
                    OnPropertyChanged("CloseRight");
                }

            }
        }

        private bool _pump;
        public bool Pump
        {
            get { return _pump; }
            set
            {
                bool oldValue = _pump;
                _pump = value;
                if (oldValue != value)
                {
                    OnPropertyChanged("Pump");
                }

            }
        }

        private bool _overPress;
        public bool OverPress
        {
            get { return _overPress; }
            set
            {
                bool oldValue = _overPress;
                _overPress = value;
                if (oldValue != value)
                    OnPropertyChanged("OverPress");
            }
        }

        private bool _overTemp;
        public bool OverTemp
        {
            get { return _overTemp; }
            set
            {
                bool oldValue = _overTemp;
                _overTemp = value;
                if (oldValue != value)
                    OnPropertyChanged("OverTemp");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}
