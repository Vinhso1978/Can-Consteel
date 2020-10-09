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
    class StatusBarViewModel: BindableBase
    {

        PlcService _service;
        System.Timers.Timer _timer;
        System.Timers.Timer _rollTimer;
        int _alarmId;
        #region Properties
        private string _sysTime;
        public string SysTime { get { return _sysTime; } set { SetProperty(ref _sysTime, value); } }

        private ObservableCollection<Alarm> _alarms = new ObservableCollection<Alarm>();
        public ObservableCollection<Alarm> Alarms
        {
            get { return _alarms; }
            set
            {
                SetProperty(ref _alarms, value);
                if (value.Count > 0)
                    _rollTimer.Start();
                else
                    _rollTimer.Stop();
            }
        }

        private Alarm _selectedAlarm;
        public Alarm SelectedAlarm { get { return _selectedAlarm; } set { SetProperty(ref _selectedAlarm, value); } }
        #endregion

        public StatusBarViewModel(PlcService service)
        {
            _service = service;
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            _rollTimer = new System.Timers.Timer(1000);
            _rollTimer.Elapsed += _rollTimer_Elapsed;
            _rollTimer.Start();
            _service.NewConnectionState1 += _service_NewConnectionState1;
            
            _service.Profinet += _service_Profinet;
        }

        private void _service_Profinet(object sender, EventArgs e)
        {
            EventProfinet p = (EventProfinet)e;
            string msg="Lỗi kết nối cân " + p.position.ToString();
            switch (p.position)
            {
                case 1:
                    if (_service.Errs.ACT350)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Lỗi kết nối cân", code = 3 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(3) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                            
                    }
                    else
                    {
                        int i = SearchAlarm(3);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(3);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 2:
                    if (_service.Errs.OpenLeft)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Lỗi không mở được cánh trái", code = 4 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(4) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                            
                    }
                    else
                    {
                        int i = SearchAlarm(4);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(4);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 3:
                    if (_service.Errs.OpenRight)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Lỗi không mở được cánh phải", code = 5 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(5) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                            
                    }
                    else
                    {
                        int i = SearchAlarm(5);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(5);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 4:
                    if (_service.Errs.CloseLeft)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Lỗi không đóng được cánh trái", code = 6 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(6) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                            
                    }
                    else
                    {
                        int i = SearchAlarm(6);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(6);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 5:
                    if (_service.Errs.CloseRight)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Lỗi không đóng được cánh phải", code = 7 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(7) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                    }
                    else
                    {
                        int i = SearchAlarm(7);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(7);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 6:
                    if (_service.Errs.Pump)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Lỗi khởi động bơm dầu", code = 8 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(8) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                    }
                    else
                    {
                        int i = SearchAlarm(8);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(8);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 7:
                    if (_service.Errs.OverPress)
                    {
                        if(SearchAlarm(9) == -1)
                        {
                            Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Cảnh báo quá áp suất dầu", code = 9 };
                            Alarms.Add(alarm);
                        }
                        
                        if (DataAccess.CheckExistAlarm(9) == 0)
                        {
                            DataAccess.InsertAlarm(new Alarm() { OccurTime = DateTime.Now, Description = "Cảnh báo quá áp suất dầu", code = 9, State=2 });
                        }
                    }
                    else
                    {
                        int i = SearchAlarm(9);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(9);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
                case 8:
                    if (_service.Errs.OverTemp)
                    {
                        Alarm alarm = new Alarm() { OccurTime = DateTime.Now, Description = "Cảnh báo quá nhiệt độ dầu", code = 10 };
                        Alarms.Add(alarm);
                        if (DataAccess.CheckExistAlarm(10) == 0)
                        {
                            alarm.State = 2;
                            DataAccess.InsertAlarm(alarm);
                        }
                    }
                    else
                    {
                        int i = SearchAlarm(10);
                        if (i > -1)
                            Alarms.RemoveAt(i);
                        long Id = DataAccess.CheckExistAlarm(10);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    break;
            }
            
        }

        private void _rollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Alarms.Count > 0)
            {
                if (_alarmId >= Alarms.Count)
                    _alarmId = 0;
                SelectedAlarm = Alarms[_alarmId];
                _alarmId++;
            }
            else
            {
                SelectedAlarm = null;
            }
        }

        private void _service_NewConnectionState1(object sender, EventArgs e)
        {
            if (_service.ConnectionState1 != 0)
                Alarms.Add(new Alarm() { OccurTime = DateTime.Now, Description = "Mất kết nối với PLC", code = 1 });
            else
            {
                int i = SearchAlarm(1);
                if (i > -1)
                    Alarms.RemoveAt(i);
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") != _sysTime)
                SysTime = DateTime.Now.ToString("dd/MM/yyyy H:mm:ss");
        }

        private int SearchAlarm(int code)
        {
            int returnValue = -1;
            for (int i = 0; i < Alarms.Count; i++)
                if (Alarms[i].code == code)
                {
                    returnValue = i;
                    break;
                }
            return returnValue;
                    
        }
    }
}
