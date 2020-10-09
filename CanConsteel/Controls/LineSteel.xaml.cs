using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CanConsteel.Controls
{
    /// <summary>
    /// Interaction logic for CalibScale.xaml
    /// </summary>
    public partial class LineSteel : UserControl
    {
        private ObservableCollection<Billet> _items = new ObservableCollection<Billet>();
        public ObservableCollection<Billet> Items { get { return _items; } set { _items = value; } }
        public LineSteel()
        {
            InitializeComponent();
            //ttt.DataContext = this;
            R2.DataContext = this;
            R1.DataContext = this;
            ItemsSteels.DataContext = Items;
        }

            
        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }

        public static readonly DependencyProperty ActiveProperty = DependencyProperty.Register("Active", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnActiveChanged));
        private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel h = (LineSteel)d;
            if (h != null)
            {
                if((bool)e.NewValue && h.Total>=0)
                    h.Items.Add(new Billet());
                else
                {
                    if (h.Items.Count > 0)
                        while (h.Items.Count > 0)
                        {
                            h.Items.RemoveAt(0);
                        }
                            
                }
            }
        }

        public long Total
        {
            get { return (long)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }
        private static readonly DependencyProperty TotalProperty = DependencyProperty.Register("Total", typeof(long), typeof(LineSteel), new PropertyMetadata());


        public bool Done
        {
            get { return (bool)GetValue(DoneProperty); }
            set
            {
                SetValue(DoneProperty, value);
            }
        }
        private static readonly DependencyProperty DoneProperty = DependencyProperty.Register("Done", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnDoneChanged));
        private static void OnDoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if((bool)e.NewValue)
                {
                    int i = lineSteel.Items.Count;
                    if (lineSteel.Items.Count > 1)
                    {
                        lineSteel.Items.RemoveAt(0);
                    }
                    
                }
            }
        }


        public bool SS6M
        {
            get { return (bool)GetValue(SS6MProperty); }
            set { SetValue(SS6MProperty, value); }
        }
        private static readonly DependencyProperty SS6MProperty = DependencyProperty.Register("SS6M", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnSS6MChanged));

        private static void OnSS6MChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if ((bool)e.NewValue)
                {
                    if (lineSteel.Items.Count > 1)
                        lineSteel.Items[0].Position = 28500 - lineSteel.Items[0].Size;
                }
            }
        }

        public bool SS12M
        {
            get { return (bool)GetValue(SS12MProperty); }
            set { SetValue(SS12MProperty, value); }
        }
        private static readonly DependencyProperty SS12MProperty = DependencyProperty.Register("SS12M", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnSS12MChanged));

        private static void OnSS12MChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if ((bool)e.NewValue)
                {
                    if (lineSteel.Items.Count > 1)
                        lineSteel.Items[0].Position = 31500 - lineSteel.Items[0].Size;
                }
            }
        }

        public bool Cut
        {
            get { return (bool)GetValue(CutProperty); }
            set { SetValue(CutProperty, value); }
        }
        public static readonly DependencyProperty CutProperty = DependencyProperty.Register("Cut", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnCutChanged));
        private static void OnCutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if ((bool)e.NewValue)
                {
                    if (lineSteel.Items.Count > 0)
                    {
                        lineSteel.Items[lineSteel.Items.Count - 1].Cutted = true;
                        lineSteel.Items[lineSteel.Items.Count - 1].RollerRun = lineSteel.RollerRun;
                        lineSteel.Items.Add(new Billet());
                    }
                    else
                        lineSteel.Items.Add(new Billet());
                }
            }
        }

        public bool RollerRun
        {
            get { return (bool)GetValue(RollerRunProperty); }
            set { SetValue(RollerRunProperty, value); }
        }
        public static readonly DependencyProperty RollerRunProperty = DependencyProperty.Register("RollerRun", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnRollerChanged));
        private static void OnRollerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if ((bool)e.NewValue)
                {
                    if (lineSteel.Items.Count > 0)
                    {
                        for(int i=0; i< lineSteel.Items.Count-1;i++)
                            lineSteel.Items[i].RollerRun = true;
                    }
                    
                }
                else
                {
                    if (lineSteel.Items.Count > 0)
                    {
                        for (int i = 0; i < lineSteel.Items.Count - 1; i++)
                            lineSteel.Items[i].RollerRun = false;
                    }
                }

            }
        }

        public bool Roller1
        {
            get { return (bool)GetValue(Roller1Property); }
            set { SetValue(Roller1Property, value); }
        }
        public static readonly DependencyProperty Roller1Property = DependencyProperty.Register("Roller1", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnRoller1Changed));
        private static void OnRoller1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if ((bool)e.NewValue)
                {
                    if (lineSteel.Items.Count > 0)
                    {
                        for (int i = 0; i < lineSteel.Items.Count - 1; i++)
                            lineSteel.Items[i].Roller1 = true;
                    }

                }
                else
                {
                    if (lineSteel.Items.Count > 0)
                    {
                        for (int i = 0; i < lineSteel.Items.Count - 1; i++)
                            lineSteel.Items[i].Roller1 = false;
                    }
                }

            }
        }

        //public bool ColorRoller1
        //{
        //    get { return (bool)GetValue(ColorRoller1Property); }
        //    set { SetValue(ColorRoller1Property, value); }
        //}

        //public static readonly DependencyProperty ColorRoller1Property = DependencyProperty.Register("ColorRoller1", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnColorRoller1Changed));
        //private static void OnColorRoller1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LineSteel h = (LineSteel)d;
        //    if (h != null)
        //    {
        //        if ((bool)e.NewValue)
        //        {
        //            if (h.Items.Count > 0)
        //            {
        //                h.Items[h.Items.Count - 1].ColorRoller1 = true;
        //            }
        //        }
                    
        //        else
        //        {
        //            if (h.Items.Count > 0)
        //            {
        //                h.Items[h.Items.Count - 1].ColorRoller1 = false;
        //            }
        //        }
        //    }
        //}

        //public bool ColorRoller2
        //{
        //    get { return (bool)GetValue(ColorRoller2Property); }
        //    set { SetValue(ColorRoller2Property, value); }
        //}

        //public static readonly DependencyProperty ColorRoller2Property = DependencyProperty.Register("ColorRoller2", typeof(bool), typeof(LineSteel), new PropertyMetadata(OnColorRoller2Changed));
        //private static void OnColorRoller2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LineSteel h = (LineSteel)d;
        //    if (h != null)
        //    {
        //        if ((bool)e.NewValue)
        //        {
        //            if (h.Items.Count > 0)
        //            {
        //                h.Items[h.Items.Count - 1].ColorRoller2 = true;
        //            }
        //        }

        //        else
        //        {
        //            if (h.Items.Count > 0)
        //            {
        //                h.Items[h.Items.Count - 1].ColorRoller2 = false;
        //            }
        //        }
        //    }
        //}

        public double BilletSize
        {
            get { return (int)GetValue(BilletSizeProperty); }
            set { SetValue(BilletSizeProperty, value); }
        }
        public static readonly DependencyProperty BilletSizeProperty = DependencyProperty.Register("BilletSize", typeof(double), typeof(LineSteel), new PropertyMetadata(OnBilletSizeChanged));
        private static void OnBilletSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel lineSteel = (LineSteel)d;
            if (lineSteel != null)
            {
                if (lineSteel.Items.Count > 0)
                {
                    if((double)e.NewValue>= lineSteel.Items[lineSteel.Items.Count - 1].Size && !lineSteel.Items[lineSteel.Items.Count-1].NewBorn)
                        lineSteel.Items[lineSteel.Items.Count-1].Size = (double)e.NewValue;
                    if (lineSteel.Items.Count > 1)
                    {
                        if(lineSteel.Items[lineSteel.Items.Count - 2].Position <= 1900)
                        {
                            lineSteel.Items[lineSteel.Items.Count - 2].Position = (double)e.NewValue;
                        }
                    }
                    
                }

            }
        }

        public bool Weighing
        {
            get { return (bool)GetValue(WeighingProperty); }
            set { SetValue(WeighingProperty, value); }
        }

        public static readonly DependencyProperty WeighingProperty = DependencyProperty.Register("Weighing", typeof(bool), typeof(LineSteel), new PropertyMetadata(WeighingChanged));
        private static void WeighingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel h = (LineSteel)d;
            if (h != null)
            {
                if ((bool)e.NewValue)
                {
                    if (h.Items.Count > 1)
                    {
                        for(int i=0; i < h.Items.Count-1; i++)
                        {
                            if(h.Items[i].Position>17500)
                                h.Items[i].ColorSteel = true;
                        }
                        
                    }
                }
                else
                {
                    for (int i = 0; i < h.Items.Count - 1; i++)
                    {
                        if (h.Items[i].Position > 17500)
                            h.Items[i].ColorSteel = false;
                    }
                }
            }
        }

        public double BilletWeight
        {
            get { return (double)GetValue(BilletWeightProperty); }
            set { SetValue(BilletWeightProperty, value); }
        }
        private static readonly DependencyProperty BilletWeightProperty = DependencyProperty.Register("BilletWeight", typeof(double), typeof(LineSteel), new PropertyMetadata(BilletWeightChanged));

        private static void BilletWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSteel h = (LineSteel)d;
            if (h != null)
            {
                if (h.Items.Count > 0)
                    h.Items[0].Weight = (double)e.NewValue;
            }
        }
    }
    public class Billet : INotifyPropertyChanged
    {
        
        private Timer _timer;
        private Timer _resetNewBorn;
        public bool NewBorn;
        private bool _rollerRun;
        public bool RollerRun
        {
            get { return _rollerRun; }
            set
            {
                _rollerRun = value;
                if (value)
                    _timer.Start();
                else
                    if(!Roller1)
                        _timer.Stop();
                OnPropertyChanged("RollerRun");
            }
        }
        private bool _roller1;
        public bool Roller1
        {
            get { return _roller1; }
            set
            {
                _roller1 = value;
                if (value)
                    _timer.Start();
                else
                    if (!RollerRun)
                        _timer.Stop();
                OnPropertyChanged("Roller1");
            }
        }
        private bool _colorSteel;
        public bool ColorSteel { get { return _colorSteel; } set { _colorSteel = value; OnPropertyChanged("ColorSteel"); } }
        private bool _cutted;
        public bool Cutted { get { return _cutted; } set { _cutted = value; OnPropertyChanged("Cutted"); } }        
        private double _size;
        public double Size { get { return _size; } set { _size = value; OnPropertyChanged("Size"); } }
        private double _position;
        public double Position { get { return _position; } set { _position = value; OnPropertyChanged("Position"); } }

        private double _weight;
        public double Weight { get { return _weight; } set { _weight = value; OnPropertyChanged("Weight"); } }

        public Billet()
        {
            _timer = new Timer(200);
            _timer.Elapsed += Timer_Elapsed;
            _resetNewBorn = new Timer(500);
            _resetNewBorn.Elapsed += _resetNewBorn_Elapsed;
            _resetNewBorn.Start();
            NewBorn = true;
        }

        private void _resetNewBorn_Elapsed(object sender, ElapsedEventArgs e)
        {
            _resetNewBorn.Stop();
            NewBorn = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(Roller1 && Position<16500)
                Position = Position + 93.333333;
            else 
                if (RollerRun && Position > 6000)
                    Position = Position + 93.333333;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string PropertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this,
                     new PropertyChangedEventArgs(PropertyName));
            }
        }
    }

    public class SizeConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int returnValue =0;
            returnValue = (int)((double)value / 31500 * 800);
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ColorRollerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returValue = "Brown";
            if ((bool)value)
            {
                returValue = "Green";
            }
            return returValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ColorSteelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returValue = "Red";
            if ((bool)value)
            {
                returValue = "Orange";
            }
            return returValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class Bool2Color : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = "Transparent";
            if ((bool)value)
                returnValue = "Red";
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility returnValue = Visibility.Hidden;
            if ((double)value > 0)
                returnValue = Visibility.Visible;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
