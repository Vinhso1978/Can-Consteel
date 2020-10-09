using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    public class NewValueEventArgs : EventArgs
    {
        public double NewValue { get; set; }
    }
}
