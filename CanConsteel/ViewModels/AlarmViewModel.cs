using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using CanConsteel.Models;
using System.Collections.ObjectModel;

namespace CanConsteel.ViewModels
{
    class AlarmViewModel:BindableBase
    {
        PlcService _plc;
        System.Timers.Timer _refreshtimer;
        #region Commands
        public DelegateCommand LoadedCommand { get; private set; }
        private void OnLoadCommand()
        {
            Alarms = DataAccess.GetAlarms();
        }

        public DelegateCommand AckCommand { get; private set; }
        private void OnAckCommand()
        {
            if(SelectedAlarm!=null)
                if(SelectedAlarm.Id>0 && SelectedAlarm.ResetTime == null)
                {
                    DataAccess.AckAlarm(SelectedAlarm.Id);
                    Alarms = DataAccess.GetAlarms();
                }
        }

        public DelegateCommand ResetCommand { get; private set; }
        private async void OnResetCommand()
        {
            if (SelectedAlarm != null)
                if (SelectedAlarm.Id > 0 && SelectedAlarm.ResetTime == null && SelectedAlarm.code>6)
                {
                    //DataAccess.AckAlarm(SelectedAlarm.Id);
                    //await _plc.Reset12M(SelectedAlarm.code);
                    _refreshtimer.Enabled = true;
                }
        }

        public DelegateCommand ClearCommand { get; private set; }
        private void OnClearCommand()
        {
            DataAccess.DeleteHistory();
            Alarms = DataAccess.GetAlarms();
        }
        #endregion

        #region Properties

        private ObservableCollection<Alarm> _alarms = new ObservableCollection<Alarm>();
        public ObservableCollection<Alarm> Alarms { get { return _alarms; } set{ SetProperty(ref _alarms, value); } }

        private Alarm _selectedAlarm = new Alarm();
        public Alarm SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                SetProperty(ref _selectedAlarm, value);
                if(SelectedAlarm == null)
                {
                    EnableAck = false;
                }
                else
                {
                    if (SelectedAlarm.ResetTime == null)
                        EnableAck = true;
                    else
                        EnableAck = false;
                }
            }
        }

        private bool _enableAck;
        public bool EnableAck { get { return _enableAck; } set { SetProperty(ref _enableAck, value); } }

        #endregion

        #region Contructor
        public AlarmViewModel(PlcService service)
        {
            _plc = service;
            Alarms = DataAccess.GetAlarms();
            LoadedCommand = new DelegateCommand(OnLoadCommand);
            AckCommand = new DelegateCommand(OnAckCommand);
            ResetCommand = new DelegateCommand(OnResetCommand);
            ClearCommand = new DelegateCommand(OnClearCommand);
            _refreshtimer = new System.Timers.Timer(1000);
            _refreshtimer.Elapsed += _refreshtimer_Elapsed;

        }

        private void _refreshtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _refreshtimer.Enabled = false;
            _refreshtimer.Stop();
            Alarms = DataAccess.GetAlarms();
        }



        #endregion
    }
}
