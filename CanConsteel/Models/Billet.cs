using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    public class Billet: INotifyPropertyChanged
    {

        public long Id { get; set; }
        public DateTime InsertTime { get; set; }
        public double Lenght { get; set; }
        public double? Weinght { get; set; }
        public int? Line { get; set; }
        public int Batch { get; set; }
        public string Grade { get; set; }
        public double ReqWeinght { get; set; }

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
