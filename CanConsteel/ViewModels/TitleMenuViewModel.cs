using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using Prism.Regions;
using CanConsteel.Models;
namespace CanConsteel.ViewModels
{
    class TitleMenuViewModel: BindableBase
    {
        private int _currentView;
        private readonly IRegionManager _regionManager;
        private LogIn _logInWindow;
        PlcService _plc;
        #region Commands
        public DelegateCommand ExitCommand { get; private set; }
        private void OnExit()
        {
            _plc.Disconnect();
            if (Properties.Settings.Default.AutoBackup && Properties.Settings.Default.BackupPath!="")
            {
                AutoBackup autobackupWindow = new AutoBackup();
                autobackupWindow.Owner = App.Current.MainWindow;
                autobackupWindow.ShowDialog();
                
            }
            App.Current.Shutdown();
        }

        public DelegateCommand HomePageCommand { get; private set; }
        private void OnHomePageCommand()
        {
            NavigatorTo("MainPage");
            _currentView = 0;
            CheckedHomePage = true;
        }

        public DelegateCommand CalibPageCommand { get; private set; }
        private void OnCalibPageCommand()
        {
            _logInWindow = new LogIn();
            _logInWindow.ShowDialog();
            if (_logInWindow.DialogResult == true)
            {
                NavigatorTo("CalibPage");
                _currentView = 1;
            }
            else
            {
                CheckedCalibPage = false;
                SetCheckRegion();
                
            }

        }

        public DelegateCommand DataPageCommand { get; private set; }
        private void OnDataPageCommand()
        {
            NavigatorTo("DataPage");
            _currentView = 2;
        }

        public DelegateCommand AlarmPageCommand { get; private set; }
        private void OnAlarmPageCommand()
        {
            NavigatorTo("AlarmPage");
            _currentView = 3;
        }
        #endregion

        #region Properties
        private ObservableCollection<Language> _languages;
        public ObservableCollection<Language> Languages { get { return _languages; } set { SetProperty(ref _languages, value); } }

        private Language _selectedLanguage;
        public Language SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                bool english;
                SetProperty(ref _selectedLanguage, value);
                if (_selectedLanguage.id == 1)
                    english = true;
                else
                    english = false;
                Properties.Settings.Default.Language = english;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();

                //App.instance.SetLanguage(english);
                SetLanguage(english);
                //States.ChangeLanguage();
                ////ColorString = States.StateAll;
                //_LanguageChanged();
                ////Machines = DataAccess.GetMachine(Properties.Settings.Default.Language);
                //_modbusService.OnLanguageChanged();
            }
        }

        private bool _checkedSettingsPage;
        public bool CheckedSettingsPage { get { return _checkedSettingsPage; } set { SetProperty(ref _checkedSettingsPage, value); } }

        private bool _checkedHomePage;
        public bool CheckedHomePage { get { return _checkedHomePage; } set { SetProperty(ref _checkedHomePage, value); } }

        private bool _checkedCalibPage;
        public bool CheckedCalibPage { get { return _checkedCalibPage; } set { SetProperty(ref _checkedCalibPage, value); } }

        private bool _checkedDataPage;
        public bool CheckedDataPage { get { return _checkedDataPage; } set { SetProperty(ref _checkedDataPage, value); } }

        private bool _checkAlarmPage;
        public bool CheckedAlarmPage { get { return _checkAlarmPage; } set { SetProperty(ref _checkAlarmPage, value); } }
        #endregion

        public TitleMenuViewModel(IRegionManager regionManager, PlcService plc)
        {
            _regionManager = regionManager;
            _plc = plc;
            ExitCommand = new DelegateCommand(OnExit);
            CalibPageCommand = new DelegateCommand(OnCalibPageCommand);
            HomePageCommand = new DelegateCommand(OnHomePageCommand);
            DataPageCommand = new DelegateCommand(OnDataPageCommand);
            AlarmPageCommand = new DelegateCommand(OnAlarmPageCommand);
            _languages = new ObservableCollection<Language>();
            Languages.Add(new Language(0, "TIẾNG VIỆT"));
            Languages.Add(new Language(1, "ENGLISH"));
            _selectedLanguage = new Language(0, "TIẾNG VIỆT");
            if (Properties.Settings.Default.Language)
                SelectedLanguage = Languages[1];
            else
                SelectedLanguage = Languages[0];
            CheckedHomePage = true;

        }

        private void NavigatorTo(string url)
        {
            _regionManager.RequestNavigate("MainRegion", url);
        }

        private void SetCheckRegion()
        {
            switch (_currentView)
            {
                case 0:
                    CheckedHomePage = true;
                    break;
                case 1:
                    CheckedCalibPage = true;
                    break;
                case 2:
                    CheckedDataPage = true;
                    break;
                case 3:
                    CheckedAlarmPage = true;
                    break;
            }
        }

        public void SetLanguage(bool englishSelected)
        {
            System.Windows.ResourceDictionary rsDct = new System.Windows.ResourceDictionary();
            if (englishSelected)
                rsDct.Source = new Uri("..\\Language\\English.xaml", UriKind.Relative);

            else
                rsDct.Source = new Uri("..\\Language\\TiengViet.xaml", UriKind.Relative);
            int langDictId = -1;
            for (int i = 0; i < App.Current.Resources.MergedDictionaries.Count; i++)
            {
                var md = App.Current.Resources.MergedDictionaries[i];
                // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName
                // key and that it is set to a value starting with "Loc-".
                if (md.Contains("Language"))
                {
                    langDictId = i;
                    break;
                }
            }
            if (langDictId == -1)
            {
                // Add in newly loaded Resource Dictionary
                App.Current.Resources.MergedDictionaries.Add(rsDct);
            }
            else
            {
                // Replace the current langage dictionary with the new one
                App.Current.Resources.MergedDictionaries[langDictId] = rsDct;
            }
        }
    }

    class Language
    {
        public Language(int i, string ten)
        {
            id = i;
            name = ten;
        }
        public int id { get; set; }
        public string name { get; set; }

    }
}
