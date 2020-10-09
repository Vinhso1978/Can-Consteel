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
using System.Windows.Shapes;

namespace CanConsteel
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = CheckPass(PasswordBox.Password);
            if (this.DialogResult == false)
                MessageBox.Show("SAI MẬT KHẨU", "THÔNG BÁO", MessageBoxButton.OK);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool CheckPass(string password)
        {
            bool returnValue = password == Properties.Settings.Default.Password ? true : false;
            return returnValue;
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = CheckPass(PasswordBox.Password);
                if (this.DialogResult == false)
                    MessageBox.Show("SAI MẬT KHẨU", "THÔNG BÁO", MessageBoxButton.OK);
                this.Close();
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    this.DialogResult = false;
                    this.Close();
                }
            }
        }
    }
}
