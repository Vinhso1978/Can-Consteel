using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    class Alarm: INotifyPropertyChanged
    {
        public long Id { get; set; }
        public int code { get; set; } 
        private DateTime _occurTime;
        public DateTime OccurTime { get { return _occurTime; } set { _occurTime = value; OnPropertyChanged("OccurTime"); } }
        private DateTime? _ackTime;
        public DateTime? AckTime { get { return _ackTime; } set { _ackTime = value; OnPropertyChanged("AckTime"); } }
        private DateTime? _resetTime;
        public DateTime? ResetTime { get { return _resetTime; } set { _resetTime = value; OnPropertyChanged("ResetTime");  } }
        private string _description;
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged("Description"); } }
        private int _state;
        public int State { get { return _state; } set { _state = value; OnPropertyChanged("State"); } }

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
