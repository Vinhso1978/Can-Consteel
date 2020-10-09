using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CanConsteel.Models
{
    class CheDoCutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = "THEO KHỐI LƯỢNG";
            if ((bool)value)
                returnValue = "THEO CHIỀU DÀI";
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CheDoDKConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue;
            if (Properties.Settings.Default.Language)
            {
                returnValue = "MANUAL";
                if ((bool)value)
                    returnValue = "AUTO";
            }
            else
            {
                returnValue = "BẰNG TAY";
                if ((bool)value)
                    returnValue = "TỰ ĐỘNG";
            }
            
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double returnValue = 60 * ((double)value) / 1000;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class AlarmStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush returnValue = new SolidColorBrush();
            returnValue.Color = Colors.Transparent;
            if ((int)value == 1)
                returnValue.Color = Colors.Yellow;
            if ((int)value == 2)
                returnValue.Color = Colors.Red;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class VisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility returnValue = Visibility.Hidden;
            if ((int)value == 2)
                returnValue = Visibility.Visible;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
