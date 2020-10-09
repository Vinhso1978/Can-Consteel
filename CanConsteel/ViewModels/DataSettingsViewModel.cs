using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CanConsteel.Models;

namespace CanConsteel.ViewModels
{
    class DataSettingsViewModel: BindableBase
    {
        System.Timers.Timer _timer = new System.Timers.Timer(1);
        string _pass1;
        string _pass2;
        #region Properties
        private string _backupPath;
        public string BackupPath { get { return _backupPath; } set { SetProperty(ref _backupPath, value); } }

        private string _restoreFile;
        public string RestoreFile { get { return _restoreFile; } set { SetProperty(ref _restoreFile, value); } }

        private double _complete;
        public double Complete { get { return _complete; } set { SetProperty(ref _complete, value); } }

        private bool _autoBackup;
        public bool AutoBackup
        {
            get { return _autoBackup; }
            set
            {
                SetProperty(ref _autoBackup, value);
            }
        }
        #endregion

        #region Command
        public DelegateCommand SelectBackup { get; private set; }
        private void OnSelectBackup()
        {
            System.Windows.Forms.FolderBrowserDialog browse = new System.Windows.Forms.FolderBrowserDialog();
            browse.SelectedPath = BackupPath;
            browse.ShowDialog();
            BackupPath = browse.SelectedPath;
            Properties.Settings.Default.BackupPath = BackupPath;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
        }

        public DelegateCommand BackupCommand { get; private set; }
        private void OnBackupCommand()
        {
            if(BackupPath!="" && Directory.Exists(BackupPath))
            {
                _timer.Start();
                Complete = 0;
                DataAccess.Backup(BackupPath);
            }
        }

        public DelegateCommand SelectFile { get; private set; }
        private void OnSelectFile()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = "c:\\";
            dialog.Filter = "backup files (*.sql)|*.sql|All files (*.*)|*.*";
            dialog.ShowDialog();
            RestoreFile = dialog.FileName;
        }

        public DelegateCommand RestoreCommand { get; private set; }
        private void OnRestoreCommand()
        {
            try
            {
                Complete = 0;
                _timer.Start();
                DataAccess.Restore(RestoreFile);
                MessageBox.Show("Phục hồi thành công cơ sở dữ liệu", "Thông báo", System.Windows.MessageBoxButton.OK);
            }
            catch
            {
                MessageBox.Show("Không phục hồi được cơ sở dữ liệu", "Báo lỗi", System.Windows.MessageBoxButton.OK);
            }
        }

        public DelegateCommand CheckedCommand { get; private set; }
        private void OnCheckedCommand()
        {
            Properties.Settings.Default.AutoBackup = true;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();

        }

        public DelegateCommand UnCheckedCommand { get; private set; }
        private void OnUnCheckedCommand()
        {
            Properties.Settings.Default.AutoBackup = false;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();

        }

        public DelegateCommand SetPassCommand { get; private set; }
        private void OnSetPassCommand()
        {
            if (_pass1 == _pass2 && _pass1 != null && _pass1 != "")
            {
                Properties.Settings.Default.Password = _pass1;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
                MessageBox.Show("ĐÃ ĐỔI THÀNH CÔNG MẬT KHẨU VẬN HÀNH", "Thông báo", System.Windows.MessageBoxButton.OK);
            }
            else
            {
                if (_pass1 == null || _pass1 == "")
                {
                    MessageBox.Show("CHƯA NHẬP MẬT KHẨU MỚI", "Lỗi", System.Windows.MessageBoxButton.OK);
                }
                if (_pass1 != _pass2)
                {
                    MessageBox.Show("MẬT KHẨU LẦN HAI KHÔNG GIỐNG LẦN MỘT", "Lỗi", System.Windows.MessageBoxButton.OK);
                }
            }
        }

        public void changePassword(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            _pass1 = passwordBox.Password;
        }

        public void changePassword2(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            _pass2 = passwordBox.Password;
        }

        #endregion

        public DataSettingsViewModel()
        {
            _timer.Elapsed += _timer_Elapsed;
            SelectBackup = new DelegateCommand(OnSelectBackup);
            BackupCommand = new DelegateCommand(OnBackupCommand);
            SelectFile = new DelegateCommand(OnSelectFile);
            RestoreCommand = new DelegateCommand(OnRestoreCommand);
            CheckedCommand = new DelegateCommand(OnCheckedCommand);
            UnCheckedCommand = new DelegateCommand(OnUnCheckedCommand);
            SetPassCommand = new DelegateCommand(OnSetPassCommand);
            AutoBackup = Properties.Settings.Default.AutoBackup;

            if (Properties.Settings.Default.BackupPath != "")
            {
                BackupPath = Properties.Settings.Default.BackupPath;
            }
            else
            {
                BackupPath =Path.Combine(Directory.GetCurrentDirectory(),"Backups");
                if (!Directory.Exists(BackupPath))
                    Directory.CreateDirectory(BackupPath);
                Properties.Settings.Default.BackupPath = BackupPath;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DataAccess.totalByte > 0)
                Complete = 100.0*(double)DataAccess.currentByte / (double)DataAccess.totalByte;
            else
                _timer.Stop();
            if (Complete == 100)
            {
                _timer.Stop();
                DataAccess.currentByte = 0;
            }
                
        }
    }
}
