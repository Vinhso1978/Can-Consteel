using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using CanConsteel.Models;
using System.Timers;
using System.Collections.ObjectModel;
//using CanConsteel.Controls;

namespace CanConsteel.ViewModels
{
    class MainPageViewModel:BindableBase
    {
        PlcService _plcService;
        bool _oldYCGhi;
        bool _read;
        bool _read2;
        bool oldFinish1;
        bool oldFinish2;
        bool oldFinish3;
        bool oldFinish4;
        bool[] scaled = new bool[4];
        int[] count = new int[4];

        #region Properties
        private bool _started;
        public bool Started { get { return _started; } set { SetProperty(ref _started, value); } }
        private bool _notStarted;
        public bool NotStarted { get { return _notStarted; } set { SetProperty(ref _notStarted, value); } }
        private bool _chedocanchinh;
        public bool ChedoCanChinh { get { return _chedocanchinh; } set { SetProperty(ref _chedocanchinh, value); } }
        private bool _chedobantd;
        public bool ChedoBanTD { get { return _chedobantd; } set { SetProperty(ref _chedobantd, value); } }
        private bool _chedoTD;
        public bool ChedoTD { get { return _chedoTD; } set { SetProperty(ref _chedoTD, value); } }

        private bool _mohet;
        public bool Mohet { get { return _mohet; } set { SetProperty(ref _mohet, value); } }

        private bool _donghet;
        public bool Donghet { get { return _donghet; } set { SetProperty(ref _donghet, value); } }

        private string _chedo;
        public string Chedo { get { return _chedo; } set { SetProperty(ref _chedo, value); } }

        private bool _ycGhi;
        public bool YCGhi { get { return _ycGhi; } set { SetProperty(ref _ycGhi, value); } }

        private double _ovenWeight;
        public double OvenWeight { get { return _ovenWeight; } set { SetProperty(ref _ovenWeight, value); } }
        private double _conveyWeight;
        public double ConveyWeight { get { return _conveyWeight; } set { SetProperty(ref _conveyWeight, value); } }
        private double _weight;
        public double Weight { get { return _weight; } set { SetProperty(ref _weight, value); } }

        private int _msMe;
        public int MSMe { get { return _msMe; } set { SetProperty(ref _msMe, value); } }

        private double _tocdoSan;
        public double TocdoSan { get { return _tocdoSan; } set { SetProperty(ref _tocdoSan, value); } }

        private string _ttBom;
        public string TTBom { get { return _ttBom; } set { SetProperty(ref _ttBom, value); } }

        private bool _bom;
        public bool Bom { get { return _bom; } set { SetProperty(ref _bom, value); } }
        #endregion

        #region Constructor
        public MainPageViewModel(PlcService plcService)
        {
            _plcService = plcService;
            _plcService.RefreshValues1 += _plcService_RefreshValues1;
            _plcService.RefreshConfig1 += _plcService_RefreshConfig1;
            SetStartCommand = new DelegateCommand(OnSetStart);
            ResetStartCommand = new DelegateCommand(OnResetStart);
            PumpStartCommand = new DelegateCommand(OnPumpStart);
            PumpStopCommand = new DelegateCommand(OnPumpStop);
            TareCommand = new DelegateCommand(OnTare);
            
        }

        #endregion

        #region Commands
        public DelegateCommand SetStartCommand { get; private set; }
        private async void OnSetStart()
        {

            await _plcService.SetStartedFlag(1);
        }

        public DelegateCommand ResetStartCommand { get; private set; }
        private async void OnResetStart()
        {

            await _plcService.ResetStartedFlag(1);
        }

        public DelegateCommand PumpStartCommand { get; private set; }
        private async void OnPumpStart()
        {

            await _plcService.WriteFlag(20*8+2,true); //M20.2
        }

        public DelegateCommand PumpStopCommand { get; private set; }
        private async void OnPumpStop()
        {
            await _plcService.WriteFlag(20*8+2,false); //M20.2
        }

        public DelegateCommand TareCommand { get; private set; }
        private async void OnTare()
        {
            await _plcService.WriteFlag(20 * 8 + 3, true); //M20.3
        }
        #endregion

        private void _plcService_RefreshConfig1(object sender, EventArgs e)
        {
            _read = true;
            
            _read = false;
        }

        private async void _plcService_RefreshValues1(object sender, EventArgs e)
        {
            OvenWeight = _plcService.ReadValue.GiaTriTrongLo;
            ConveyWeight = _plcService.ReadValue.GiaTriBangTai;
            Weight = _plcService.ReadValue.GiaTriCan;
            Started = _plcService.ReadValue.Started;
            NotStarted = !_plcService.ReadValue.Started;
            ChedoCanChinh = _plcService.ReadValue.Chedocanchinh;
            ChedoBanTD = _plcService.ReadValue.Chedobantd;
            ChedoTD = _plcService.ReadValue.Chedotd;
            TocdoSan = _plcService.ReadValue.TocdoSan;
            if (ChedoCanChinh)
                Chedo = "BẰNG TAY";
            else
                Chedo = "TỰ ĐỘNG";
            Mohet = _plcService.ReadValue.Mohet;
            Donghet = _plcService.ReadValue.Donghet;
            MSMe = _plcService.ReadValue.MSMe;
            YCGhi = _plcService.ReadValue.YCGhi;
            if (YCGhi && !_oldYCGhi)
            {
                Batch newBatch = new Batch() { BatchId = MSMe, Weinght = 0, StartTime = DateTime.Now };
                DataAccess.InsertBatch(newBatch);
                DataAccess.UpdateBatchWeinght(_plcService.ReadValue.MSMeCu, _plcService.ReadValue.GiaTriMe);
                //DataAccess.InsertBillet()
                await _plcService.ResetYCGhiFlag(1);
            }
            _oldYCGhi = YCGhi;
            Bom = _plcService.ReadValue.Bom;

        }
    }
}
