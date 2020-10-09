using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CanConsteel.Models;
using CanConsteel.Views;

namespace CanConsteel.ViewModels
{
    class DataPageViewModel: BindableBase
    {
        DateTime _startTime;
        DateTime _stopTime;
        private LogIn _logInWindow;
        #region Properties
        private int _filterId;
        public int FilterId
        {
            get { return _filterId; }
            set
            {
                SetProperty(ref _filterId, value);
            }
        }

        private DateTime _selectDate1 = new DateTime();
        public DateTime SelectDate1 { get { return _selectDate1; } set { SetProperty(ref _selectDate1, value); } }

        private DateTime _selectDate2 = new DateTime();
        public DateTime SelectDate2 { get { return _selectDate2; } set { SetProperty(ref _selectDate2, value); } }

        private ObservableCollection<Batch> _batchs = new ObservableCollection<Batch>();
        public ObservableCollection<Batch> Batchs { get { return _batchs; } set { SetProperty(ref _batchs, value); } }

        private int _total;
        public int Total { get { return _total; } set { SetProperty(ref _total, value); } }

        private double _totalWeinght;
        public double TotalWeinght { get { return _totalWeinght; } set { SetProperty(ref _totalWeinght, value); } }
        #endregion

        #region Command
        public DelegateCommand UpdateCommand { get; private set; }
        private void OnUpdateCommand()
        {
            if(FilterId == 0)
            {
                _startTime = SelectDate1;
                _stopTime = SelectDate1.AddDays(1);
                
            }
            if(FilterId == 1)
            {
                _startTime = new DateTime(SelectDate1.Year, SelectDate1.Month, 1);
                _stopTime = _startTime.AddMonths(1);
                
            }
            if(FilterId == 2)
            {
                if (SelectDate2 > SelectDate1)
                {
                    _startTime = SelectDate1;
                    _stopTime = SelectDate2;
                }
                else
                {
                    MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            double totalWeinght = 0 ;
            Batchs = DataAccess.GetBatchs(_startTime, _stopTime,ref totalWeinght);
            Total = Batchs.Count;
            TotalWeinght = totalWeinght;
        }

        //public DelegateCommand ReportCommand { get; private set; }
        //private void OnReportCommand()
        //{
        //    Report report = new Report(Billets,SelectDate1);
        //    report.Owner = App.Current.MainWindow;
        //    report.ShowDialog();
            
        //}

        public DelegateCommand BackUpCommand { get; private set; }
        private void OnBackupCommand ()
        {
            _logInWindow = new LogIn();
            _logInWindow.ShowDialog();
            if (_logInWindow.DialogResult == true)
            {
                DataSettings dataSettings = new DataSettings();
                dataSettings.Owner = App.Current.MainWindow;
                dataSettings.ShowDialog();
            }
            
        }
        #endregion

        public DataPageViewModel()
        {
            UpdateCommand = new DelegateCommand(OnUpdateCommand);
            //ReportCommand = new DelegateCommand(OnReportCommand);
            BackUpCommand = new DelegateCommand(OnBackupCommand);
            SelectDate1 = DateTime.Now;
            SelectDate2 = SelectDate1.AddDays(1);
            FilterId = 0;
        }
    }
}
