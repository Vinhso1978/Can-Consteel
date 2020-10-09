using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanConsteel.Models
{
    class CutModeCaption
    {
        public bool Value { get; set; }
        private string _viCaption;
        private string _enCaption;
        public string Caption
        {
            get
            {
                if (Properties.Settings.Default.Language)
                    return _enCaption;
                else
                    return _viCaption;
            }
        }
        public CutModeCaption(bool value, string tiengviet, string english)
        {
            Value = value;
            _viCaption = tiengviet;
            _enCaption = english;
        }
    }
}
