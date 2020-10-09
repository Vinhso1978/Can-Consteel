using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CanConsteel.Views;
namespace CanConsteel.ViewModels
{
    class MainWindowViewModel: BindableBase
    {
        private readonly IRegionManager _regionManager;
        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(MainPage));
            _regionManager.RegisterViewWithRegion("TitleMenuRegion", typeof(TitleMenu));
            _regionManager.RegisterViewWithRegion("StatusBarRegion", typeof(StatusBar));
        }

    }
}
