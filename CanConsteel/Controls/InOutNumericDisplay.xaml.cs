using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CanConsteel.Models;
using Xceed.Wpf.Toolkit;

namespace CanConsteel.Controls
{
    /// <summary>
    /// Interaction logic for InOutNumericDisplay.xaml
    /// </summary>
    public partial class InOutNumericDisplay : UserControl
    {
        public bool _editting;
        private DispatcherTimer Delay = new DispatcherTimer();
        public event EventHandler<NewValueEventArgs> FinishEdit;
        private void OnFinishEdit(NewValueEventArgs e)
        {
            EventHandler<NewValueEventArgs> handler = FinishEdit;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public InOutNumericDisplay()
        {
            InitializeComponent();
            InOut.Value = 0;
            InOut.FormatString = FormatString;
            
            Delay.Interval = new TimeSpan(0, 0, 2);
            Delay.Tick += Delay_Tick;
        }

        private void Delay_Tick(object sender, EventArgs e)
        {
            _editting = false;
            InOut.Value = (double)GetValue(ValueProperty);
            Delay.Stop();
        }
        private void DoubleUpDown_LostFocus(object sender, RoutedEventArgs e)
        {
            Delay.Start();
        }

        private void DoubleUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            _editting = true;
            
        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata(OnValueChanged));
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InOutNumericDisplay inOutNumericDisplay = (InOutNumericDisplay)d;
            if (inOutNumericDisplay != null)
            {
                if (!inOutNumericDisplay._editting)
                    inOutNumericDisplay.InOut.Value = (double)e.NewValue;
            }
        }

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata(OnHeightChanged));
        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InOutNumericDisplay inOutNumericDisplay = (InOutNumericDisplay)d;
            if (inOutNumericDisplay != null)
            {
                inOutNumericDisplay.InOut.Height = (double)e.NewValue;
                inOutNumericDisplay.OK.Height = (double)e.NewValue;
            }
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata(OnWidthChanged));
        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InOutNumericDisplay inOutNumericDisplay = (InOutNumericDisplay)d;
            if (inOutNumericDisplay != null)
            {
                if (inOutNumericDisplay.HaveButton)
                {
                    inOutNumericDisplay.InOut.Width = ((double)e.NewValue - 80)>20? (double)e.NewValue-80:20;
                    inOutNumericDisplay.OK.Width = 75;
                }
                else
                {
                    inOutNumericDisplay.InOut.Width = inOutNumericDisplay.Width;
                    inOutNumericDisplay.OK.Width = 0;
                }
            }
        }

        public bool HaveButton
        {
            get { return (bool)GetValue(HaveButtonProperty); }
            set { SetValue(HaveButtonProperty, value); }
        }
        public static readonly DependencyProperty HaveButtonProperty = DependencyProperty.Register("HaveButton", typeof(bool), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata(OnHaveButtonChanged));
        private static void OnHaveButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InOutNumericDisplay inOutNumericDisplay = (InOutNumericDisplay)d;
            if (inOutNumericDisplay != null)
            {
                if ((bool)e.NewValue)
                {
                    inOutNumericDisplay.InOut.Width = (inOutNumericDisplay.Width - 80) > 20 ? inOutNumericDisplay.Width - 80 : 20;
                    inOutNumericDisplay.OK.Width = 75;
                }
                else
                {
                    inOutNumericDisplay.InOut.Width = inOutNumericDisplay.Width;
                    inOutNumericDisplay.OK.Width = 0;
                }    
                
            }
        }

        public double? Maximum
        {
            get
            {
                return (double?)GetValue(MaximumProperty);
            }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double?), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata(null)); 

        public double? Minimum
        {
            get
            {
                return (double?)GetValue(MinimumProperty);
            }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double?), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata(null)); 

        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }
        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(InOutNumericDisplay), new FrameworkPropertyMetadata("# ##0.0", OnFormatStringChanged));
        private static void OnFormatStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InOutNumericDisplay inOutNumericDisplay = (InOutNumericDisplay)d;
            if(inOutNumericDisplay != null)
            {
                inOutNumericDisplay.InOut.FormatString = (string)e.NewValue;
            }
        }
        
        private void InOut_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                DoubleUpDown s = (DoubleUpDown)sender;
                UpdateNewValue((double)s.Value);
                OK.Focus();
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    _editting = false;
                    OK.Focus();
                }
                    
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            UpdateNewValue((double)InOut.Value);
        }

        private void UpdateNewValue(double newValue)
        {
            if (Maximum == null || newValue <= Maximum)
            {
                if (Minimum == null || newValue >= Minimum)
                {
                    NewValueEventArgs newValueArg = new NewValueEventArgs() { NewValue = newValue };
                    OnFinishEdit(newValueArg);
                }
                else
                {
                    string msg = "Minimum = " + Minimum.ToString();
                    System.Windows.MessageBox.Show(msg, "Giá trị nhập quá nhỏ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                string msg = "Maximum = " + Maximum.ToString();
                System.Windows.MessageBox.Show(msg, "Giá trị nhập quá lớn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            _editting = false;

        }
    }

}
