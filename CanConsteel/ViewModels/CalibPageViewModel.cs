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
    class CalibPageViewModel: BindableBase
    {
        PlcService _plcService;
        private int _index=0;
        
        private bool _ready2Next;
        
        #region Command
        #region 1
        public DelegateCommand ZeroCommand1 { get; private set; }
        private async void OnZeroCommand1()
        {
            
            await _plcService.SetZeroFlag(1);
        }

        public DelegateCommand PointsCommand1 { get; private set; }
        private async void OnPointsCommand1()
        {
            await _plcService.SetNumOfPointsFlag(1);
            SpanValues1[0].Active = true;
            _index = 1;
            for (int i=1; i < SpanValues1.Count; i++)
            {
                SpanValues1[i].Active = false;
            }
            
        }

        public DelegateCommand ConfirmCommand1 { get; private set; }
        private async void OnConfirmCommand1()
        {
            await _plcService.SetConfirmFlag(1);
        }
        
        public DelegateCommand CancelCommand1 { get; private set; }
        private async void OnCancelCommand1()
        {
            await _plcService.SetCancelFlag(1);
            _index = 0;
        }
        #endregion

        #endregion

        #region Property
        #region 1
        private bool _dataOK;
        public bool DataOK { get { return _dataOK; } set { SetProperty(ref _dataOK, value); } }

        private bool _enableZero1;
        public bool EnableZero1 { get { return _enableZero1; } set { SetProperty(ref _enableZero1, value); } }

        private string _scaleName1;
        public string ScaleName1 { get { return _scaleName1; } set { SetProperty(ref _scaleName1, value); } }

        private double _currentValue1;
        public double CurrentValue1 { get { return _currentValue1; } set { SetProperty(ref _currentValue1, value); } }

        private int _status1;
        public int Status1 { get { return _status1; } set { SetProperty(ref _status1, value); } }

        private int _numOfPoints1;
        public int NumOfPoints1 { get { return _numOfPoints1; } set { SetProperty(ref _numOfPoints1, value); } }

        private string _calibMsg1;
        public string CalibMsg1 { get { return _calibMsg1; } set { SetProperty(ref _calibMsg1, value); } }

        private ObservableCollection<SpanPoint> _spanValues1 = new ObservableCollection<SpanPoint>();
        public ObservableCollection<SpanPoint> SpanValues1 { get { return _spanValues1; } set { SetProperty(ref _spanValues1, value); } }

        #endregion

        #endregion

        public CalibPageViewModel(PlcService plcService)
        {
            _plcService = plcService;
            _plcService.RefreshValues1 += _plcService_RefreshValues1;
            _plcService.RefreshConfig1 += _plcService_RefreshConfig1;
            
            ZeroCommand1 = new DelegateCommand(OnZeroCommand1);
            PointsCommand1 = new DelegateCommand(OnPointsCommand1);
            ConfirmCommand1 = new DelegateCommand(OnConfirmCommand1);
            CancelCommand1 = new DelegateCommand(OnCancelCommand1);

            this.PropertyChanged += CalibPageViewModel_PropertyChanged;
            ScaleName1 = "LINE 1";
            //Ratio1 = _plcService.ConfigValue1.LenRatio;
            //Ratio2 = _plcService.ConfigValue2.LenRatio;
            //Ratio3 = _plcService.ConfigValue3.LenRatio;
            //Ratio4 = _plcService.ConfigValue4.LenRatio;
            //EnableZero1 = true;
        }

        
        private void _plcService_RefreshConfig1(object sender, EventArgs e)
        {
            //Ratio1 = _plcService.ConfigValue1.LenRatio;
            //Ratio2 = _plcService.ConfigValue2.LenRatio;
        }

        private void _plcService_RefreshConfig2(object sender, EventArgs e)
        {
            //Ratio3 = _plcService.ConfigValue3.LenRatio;
            //Ratio4 = _plcService.ConfigValue4.LenRatio;
        }

        private void _plcService_RefreshValues1(object sender, EventArgs e)
        {
            if ((_plcService.ReadValue.TrangThaiCan & 8) == 0)
                DataOK = false;
            else
                DataOK = true;

            Status1 = _plcService.ReadValue.TrangThaiLenh;
            
            switch (Status1)
            {
                case 0:
                    if (_index == 0)
                    {
                        if (Properties.Settings.Default.Language)
                            CalibMsg1 = "Ready";
                        else
                            CalibMsg1 = "Sẵn sàng";
                    }
                    _ready2Next = false;
                    break;
                case 2047:
                    if (Properties.Settings.Default.Language)
                        CalibMsg1 = "Proccessing...";
                    else
                        CalibMsg1 = "Đang thực hiện...";

                    _ready2Next = false;
                    break;
                case 2045:
                    if (!_plcService.ReadValue.LoiGhi)
                    {
                        if (Properties.Settings.Default.Language)
                            CalibMsg1 = "Next point! Change weight on the scale then press OK";
                        else
                            CalibMsg1 = "Căn chỉnh điểm tiếp theo! Đặt cân chuẩn rồi nhấn nút Xong";
                        if (!_ready2Next)
                        {
                            _index++;
                            _ready2Next = true;
                        }
                        for (int i = 0; i < SpanValues1.Count; i++)
                        {
                            if (i == _index - 1)
                                SpanValues1[i].Active = true;
                            else
                                SpanValues1[i].Active = false;

                        }
                    }
                    break;
                case 2046:
                    if (!_plcService.ReadValue.LoiGhi)
                        if (Properties.Settings.Default.Language)
                            CalibMsg1 = "Done! Please confirm.";
                        else
                            CalibMsg1 = "Kết thúc! Nhấn nút Xác nhận";
                    for (int i = 0; i < SpanValues1.Count; i++)
                    {
                        SpanValues1[i].Active = false;
                    }
                    break;
            }
            
            if (Status1 == 2047)
                EnableZero1 = false;
            else
                EnableZero1 = true;
            
            CurrentValue1 = _plcService.ReadValue.GiaTriCanRaw;

        }

        private async void CalibPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(NumOfPoints1))
            {
                await _plcService.SetNumOfPoint(1,NumOfPoints1-1);
                while(SpanValues1.Count != NumOfPoints1)
                {
                    if (SpanValues1.Count > NumOfPoints1)
                        SpanValues1.RemoveAt(SpanValues1.Count-1);
                    else
                        SpanValues1.Add(new SpanPoint(_plcService) { Active = false, ScaleID=1, Id = SpanValues1.Count+1, SpanValue = 0});
                }
            }

            
        }

        
    }
}
