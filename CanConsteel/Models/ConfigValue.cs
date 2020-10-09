using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    class ConfigValue
    {
        public int ScaleID;
        public bool ReqZero;        //2.0
        public bool ReqSpanNumber;  //2.1
        public bool ReqSpan;        //2.2
        public bool ReqFinish;      //2.3
        public bool ReqCancel;      //2.4
        public int SpanNumber;      //4
        public double SpanValue;    //6
        public bool Started;        //10.1
    }
}
