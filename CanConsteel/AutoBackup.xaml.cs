using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using CanConsteel.Models;
namespace CanConsteel
{
    /// <summary>
    /// Interaction logic for AutoBackup.xaml
    /// </summary>
    public partial class AutoBackup : Window
    {
        DispatcherTimer _timer = new DispatcherTimer();
        BackgroundWorker _backupWorker;
        public AutoBackup()
        {
            InitializeComponent();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timer.Tick += _timer_Tick;
            _timer.Start();
            _backupWorker = new BackgroundWorker();
            _backupWorker.DoWork += _backupWorker_DoWork;
            _backupWorker.RunWorkerCompleted += _backupWorker_RunWorkerCompleted;
            _backupWorker.RunWorkerAsync();
            
        }

        private void _backupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataAccess.currentByte = DataAccess.totalByte;
        }

        private void _backupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataAccess.Backup(Properties.Settings.Default.BackupPath);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (DataAccess.totalByte > 0)
                Progress.Value = 100.0 * (double)DataAccess.currentByte / (double)DataAccess.totalByte;
            else
                _timer.Stop();
            if (Progress.Value == 100)
            {
                _timer.Stop();
                DataAccess.currentByte = 0;
                this.Close();
            }
        }

        
    }
}
