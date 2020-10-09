using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    class SpanPoint : INotifyPropertyChanged
    {
        PlcService _plc;

        public SpanPoint(PlcService plc)
        {
            _plc = plc;
        }

        private bool _active;
        public bool Active { get { return _active; } set { _active = value; OnPropertyChanged("Active"); } }

        private int _scaleID;
        public int ScaleID { get { return _scaleID; } set { _scaleID = value; OnPropertyChanged("ScaleID"); } }

        private int _id;
        public int Id { get { return _id; } set { _id = value; OnPropertyChanged("Id"); } }

        private double _spanValue;
        public double SpanValue
        {
            get { return _spanValue; }
            set
            {
                _spanValue = value;
                OnPropertyChanged("SpanValue");
                if(Active)
                    Task.Run(async()=> await _plc.SetSpanValue(ScaleID, SpanValue));
            }
        }

        private DelegateCommand _command;
        public DelegateCommand Command { get { return _command ?? (_command = new DelegateCommand(OnCommand)); } }

        private async void OnCommand()
        {
            await _plc.SetSpanFlag(ScaleID);
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
