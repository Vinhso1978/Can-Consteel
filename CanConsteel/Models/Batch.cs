using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    public class Batch : INotifyPropertyChanged
    {

        public long Id { get; set; }
        public long BatchId { get; set; }
        public double? Weinght { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; } 
       
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
