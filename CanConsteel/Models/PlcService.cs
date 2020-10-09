using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp7;
//using Sharp7.S7Client;
namespace CanConsteel.Models
{
    class PlcService
    {
        S7Client _plc1;
        S7Client _plc2;
        

        bool _firtReadPLC1;

        System.Timers.Timer _updatetimer1;
        System.Timers.Timer _reconnection1;

        private volatile object _locker = new object();
        double _updateTime = 100;

        bool _calib;

        #region Events
        public event EventHandler NewConnectionState1;
        public void OnNewConnectionState1()
        {
            NewConnectionState1?.Invoke(this, new EventArgs());
        }

        public event EventHandler NewConnectionState2;
        public void OnNewConnectionState2()
        {
            NewConnectionState2?.Invoke(this, new EventArgs());
        }

        public event EventHandler RefreshValues1;
        public void OnRefreshValues1()
        {
            RefreshValues1?.Invoke(this, new EventArgs());
        }

        public event EventHandler RefreshValues2;
        public void OnRefreshValues2()
        {
            RefreshValues2?.Invoke(this, new EventArgs());
        }

        public event EventHandler RefreshConfig1;
        public void OnRefreshConfig1()
        {
            RefreshConfig1?.Invoke(this, new EventArgs());
        }

        public event EventHandler RefreshConfig2;
        public void OnRefreshConfig2()
        {
            RefreshConfig2?.Invoke(this, new EventArgs());
        }

        public event EventHandler Profinet;
        public void OnProfinet(int position)
        {
            EventProfinet pos = new EventProfinet() { position = position };
            Profinet?.Invoke(this, pos);
        }
        #endregion

        #region Properties
        private int _connectionState1 = -1;
        public int ConnectionState1
        {
            get { return _connectionState1; }
            set
            {
                int oldValue = _connectionState1;
                _connectionState1 = value;
                if (oldValue != value && (oldValue == 0 || value == 0 || oldValue == -1))
                {
                    if (value != 0)
                    {
                        if (DataAccess.CheckExistAlarm(1) == 0)
                            DataAccess.InsertAlarm(new Alarm() { code = 1, OccurTime = DateTime.Now, Description = "Không kết nối được với PLC ", State=2 });
                    }
                    else
                    {
                        long Id = DataAccess.CheckExistAlarm(1);
                        if (Id > 0)
                            DataAccess.ResetAlarm(Id);
                    }
                    OnNewConnectionState1();
                }
            }
        }


        private ScaleStateValue _readValue = new ScaleStateValue();
        public ScaleStateValue ReadValue { get { return _readValue; } set { _readValue = value; } }

        private ConfigValue _configValue1 = new ConfigValue();
        public ConfigValue ConfigValue1 { get { return _configValue1; } set { _configValue1 = value; } }

        public double Empty { get; set; }

        private Errors _errs = new Errors();
        public Errors Errs
        {
            get { return _errs; }
            set
            {
                _errs = value;
            }
        }

        
        #endregion

        #region Contructor

        public PlcService()
        {
            _updatetimer1 = new System.Timers.Timer();
            _updatetimer1.Interval = _updateTime;
            _updatetimer1.Elapsed += _updatetimer1_Elapsed;
            _updatetimer1.Enabled = false;

            _reconnection1 = new System.Timers.Timer();
            _reconnection1.Interval = 5000;
            _reconnection1.Elapsed += _reconnection1_Elapsed;

            _plc1 = new S7Client();
            int ConnectionState1 = _plc1.ConnectTo("192.168.0.9", 0, 1);
            if (ConnectionState1 == 0)
            {
                _updatetimer1.Start();
                _firtReadPLC1 = true;
            }
                
            else
                _reconnection1.Start();

            Errs.PropertyChanged += ProfinetErr_PropertyChanged;

        }

        #endregion

        private void ProfinetErr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Errs.ACT350):
                    OnProfinet(1);
                    break;
                case nameof(Errs.OpenLeft):
                    OnProfinet(2);
                    break;
                case nameof(Errs.OpenRight):
                    OnProfinet(3);
                    break;
                case nameof(Errs.CloseLeft):
                    OnProfinet(4);
                    break;
                case nameof(Errs.CloseRight):
                    OnProfinet(5);
                    break;
                case nameof(Errs.Pump):
                    OnProfinet(6);
                    break;
                case nameof(Errs.OverPress):
                    OnProfinet(7);
                    break;
                case nameof(Errs.OverTemp):
                    OnProfinet(8);
                    break;
            }

        }

        public int ReadPLCValue()
        {
            int returnValue = 0;
            byte[] Buffer = new byte[42];
            lock (_locker)
            {
                ConnectionState1 = _plc1.DBRead(1, 0, 42, Buffer);
                if (ConnectionState1 == 0)
                {
                    ReadValue = ReadState(Buffer, 0);
                    byte byte1 = S7.GetByteAt(Buffer, 40);
                    Errs.ACT350 = ((byte1 & 1) == 0) ? false : true;
                    Errs.OpenLeft = ((byte1 & 2) == 0) ? false : true;
                    Errs.OpenRight = ((byte1 & 4) == 0) ? false : true;
                    Errs.CloseLeft = ((byte1 & 8) == 0) ? false : true;
                    Errs.CloseRight = ((byte1 & 16) == 0) ? false : true;
                    Errs.Pump = ((byte1 & 32) == 0) ? false : true;
                    Errs.OverPress = ((byte1 & 64) == 0) ? false : true;
                    Errs.OverTemp = ((byte1 & 128) == 0) ? false : true;
                }
                else
                {
                    returnValue = ConnectionState1;
                }
            }
            return returnValue;
        }


        #region Đọc các giá trị cài đặt (khi bắt đầu chạy chương trình)
        public int ReadConfigValue1()
        {
            int returnValue = 0;
            byte[] Buffer = new byte[50];
            lock (_locker)
            {
                ConnectionState1 = _plc1.DBRead(2, 0, 30, Buffer);
                if (ConnectionState1 == 0)
                {

                    ConfigValue1 = ReadConfig(Buffer, 0);
                    Empty = S7.GetRealAt(Buffer, 20);
                }
                else
                {
                    returnValue = ConnectionState1;
                }
            }
            return returnValue;
        }
        #endregion

        public async Task<int> WriteFlag(int pos, bool value)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, value);
                        int returnValue = 0;
                        returnValue = _plc1.WriteArea(S7Consts.S7AreaMK, 2, pos, 1, S7Consts.S7WLBit, Buffer);
                        return returnValue;
                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetZeroFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue=0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 16, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 34, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                                //break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;
                            
                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetNumOfPointsFlag(int pos)
        {
            int result = 0;
            if ((ConnectionState1 == 0 && pos <= 2) )
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 17, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 195, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 35, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 195, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetSpanFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 18, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 196, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 36, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 196, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetConfirmFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0 )
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 19, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 197, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 37, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 197, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
                
            return result;
        }

        public async Task<int> SetCancelFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 20, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 198, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 38, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 198, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetNumOfPoint(int pos, int writeValue)
        {
            int result = 0;
            if (ConnectionState1 == 0 )
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[2];
                        S7.SetIntAt(Buffer, 0, (short)writeValue);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 4, 1, S7Consts.S7WLInt, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 26, 1, S7Consts.S7WLInt, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 6, 1, S7Consts.S7WLInt, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 26, 1, S7Consts.S7WLInt, Buffer);
                            //    break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetSpanValue(int pos, double writeValue)
        {
            int result = 0;
            if (ConnectionState1 == 0 )
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[4];
                        S7.SetRealAt(Buffer, 0, (float)writeValue);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 6, 1, S7Consts.S7WLReal, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 28, 1, S7Consts.S7WLReal, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 8, 1, S7Consts.S7WLReal, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 28, 1, S7Consts.S7WLReal, Buffer);
                            //    break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> SetStartedFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 81, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 34, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> ResetStartedFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, false);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 81, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 34, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> ResetYCGhiFlag(int pos)
        {
            int result = 0;
            if (ConnectionState1 == 0)
            {
                result = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[1];
                        S7.SetBitAt(ref Buffer, 0, 0, true);
                        int returnValue = 0;
                        switch (pos)
                        {
                            case 1:
                                returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 80, 1, S7Consts.S7WLBit, Buffer);
                                break;
                            //case 2:
                            //    returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 3:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 34, 1, S7Consts.S7WLBit, Buffer);
                            //    break;
                            //case 4:
                            //    returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 2, 194, 1, S7Consts.S7WLBit, Buffer);
                            //break;
                            default:
                                returnValue = 0;
                                break;
                        }
                        return returnValue;

                    }
                });
            }
            else
                result = ConnectionState1;
            return result;
        }

        public async Task<int> Setpoint(bool len, double writeValue)
        {
            int result1 = 0;
            int result2 = 0;
            if (ConnectionState1 == 0 )
            {
                result1 = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[4];
                        S7.SetRealAt(Buffer, 0, (float)writeValue);
                        int returnValue = 0;
                        if(len)
                            returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 44, 1, S7Consts.S7WLReal, Buffer);
                        else
                            returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 2, 40, 1, S7Consts.S7WLReal, Buffer);
                        return returnValue;

                    }
                });
            }
            int result = 0;
            if (result1 == 0 && result2 == 0)
            {
                result = 0;
                _firtReadPLC1 = true;

            }
            else
            {
                if (result1 != 0)
                    result = result1;
                else
                    result = result2;
            }
            return result;
        }

        public async Task<int> SetpointPerChannel(int pos, double writeValue)
        {
            int result1 = 0;
            if ((pos<3 && ConnectionState1 == 0))
            {
                result1 = await Task.Run(() =>
                {
                    lock (_locker)
                    {
                        byte[] Buffer = new byte[4];
                        S7.SetRealAt(Buffer, 0, (float)writeValue);
                        int returnValue = 0;
                        if (pos<3)
                            returnValue = _plc1.WriteArea(S7Consts.S7AreaDB, 1, 34*(pos-1)+30, 1, S7Consts.S7WLReal, Buffer);
                        else
                            returnValue = _plc2.WriteArea(S7Consts.S7AreaDB, 1, 34*(pos-3)+30, 1, S7Consts.S7WLReal, Buffer);
                        return returnValue;

                    }
                });
            }
            
            return result1;
        }

        private void _reconnection1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _reconnection1.Stop();
            ConnectionState1 = _plc1.ConnectTo("192.168.0.9", 0, 1);
            if (ConnectionState1 == 0)
            {
                _updatetimer1.Start();
                _firtReadPLC1 = true;
            }
                
            else
            {
                _plc1.Disconnect();
                _reconnection1.Start();
            }
                
        }

        private void _updatetimer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _updatetimer1.Stop();
            if (_firtReadPLC1)
            {
                // Đọc lại giá trị cài đặt
                ConnectionState1 = ReadConfigValue1();
                if (ConnectionState1 == 0)
                    OnRefreshConfig1();
                // Đọc các trạng thái
                if (ConnectionState1 == 0)
                    ConnectionState1 = ReadPLCValue();
                _firtReadPLC1 = false;
            }
            else
            {
                if (ConnectionState1 == 0)
                    ConnectionState1 = ReadPLCValue();
            }
            if (ConnectionState1 == 0)
            {
                OnRefreshValues1();
                _updatetimer1.Start();
            }
            else
            {
                _plc1.Disconnect();
                _reconnection1.Start();
            }    

        }

        public void Disconnect()
        {
            if (_plc1 != null)
            {
                _updatetimer1.Stop();
                _plc1.Disconnect();
            }
        }

        #region Đọc các trạng thái
        private ScaleStateValue  ReadState(byte[] Buffer, int start)
        {
            ScaleStateValue returnValue = new ScaleStateValue();
            returnValue.LoiGhi = S7.GetBitAt(Buffer, start, 0);
            returnValue.Started = S7.GetBitAt(Buffer, start, 1);
            returnValue.Chedocanchinh = S7.GetBitAt(Buffer, start, 2);
            returnValue.Chedobantd = S7.GetBitAt(Buffer, start, 3);
            returnValue.Chedotd = S7.GetBitAt(Buffer, start, 4);
            returnValue.Mohet = S7.GetBitAt(Buffer, start, 5);
            returnValue.Donghet = S7.GetBitAt(Buffer, start, 6);
            returnValue.Dangmo = S7.GetBitAt(Buffer, start, 7);
            returnValue.YCGhi = S7.GetBitAt(Buffer, start + 1, 1);
            returnValue.Bom = S7.GetBitAt(Buffer, start + 1, 2);
            returnValue.TrangThaiCan = S7.GetIntAt(Buffer, start + 2);
            returnValue.TrangThaiLenh = S7.GetIntAt(Buffer, start + 4);
            returnValue.GiaTriCan = S7.GetRealAt(Buffer, start + 6);
            returnValue.GiaTriBangTai = S7.GetRealAt(Buffer, start + 10);
            returnValue.GiaTriTrongLo = S7.GetRealAt(Buffer, start + 14);
            returnValue.GiaTriMe = S7.GetRealAt(Buffer, start + 18);
            returnValue.MSMe = S7.GetIntAt(Buffer, start + 22);
            returnValue.MSMeCu = S7.GetIntAt(Buffer, start + 24);
            returnValue.TocdoSan = S7.GetRealAt(Buffer, start + 26);
            returnValue.GiaTriCanRaw = S7.GetRealAt(Buffer, start + 30);
            return returnValue;
        }

        #endregion

        private ConfigValue ReadConfig(byte[] Buffer, int start)
        {
            ConfigValue returnValue = new ConfigValue();
            //returnValue.Started = S7.GetBitAt(Buffer, 81, 1);
            return returnValue;
        }
    }
    public class EventProfinet: EventArgs
    {
        public int position;
    }
}
