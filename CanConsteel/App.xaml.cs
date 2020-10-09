using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Unity;
using CanConsteel.Views;
using CanConsteel.Models;
namespace CanConsteel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<PlcService>();
            containerRegistry.RegisterForNavigation<MainPage>("MainPage");
            containerRegistry.RegisterForNavigation<CalibPage>("CalibPage");
            containerRegistry.RegisterForNavigation<DataPage>("DataPage");
            containerRegistry.RegisterForNavigation<Views.Alarm>("AlarmPage");
            containerRegistry.RegisterForNavigation<TitleMenu>("TitleMenu");
            containerRegistry.RegisterForNavigation<StatusBar>("StatusBar");
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            
            //containerRegistry.RegisterForNavigation<MainPage>("MainPage");

        }
        protected override Window CreateShell()
        {
            SplashScreen splash = new SplashScreen();
            splash.Show();
            MainWindow mainwindow = new MainWindow();
            mainwindow.Dispatcher.BeginInvoke((Action)delegate
            {
               mainwindow.Show();
               splash.Close();
            });
            return mainwindow;
        }

        public void SetLanguage(bool englishSelected)
        {
            System.Windows.ResourceDictionary rsDct = new System.Windows.ResourceDictionary();
            if (englishSelected)
                rsDct.Source = new Uri("..\\Language\\English.xaml", UriKind.Relative);

            else
                rsDct.Source = new Uri("..\\Language\\TiengViet.xaml", UriKind.Relative);
            int langDictId = -1;
            for (int i = 0; i < Resources.MergedDictionaries.Count; i++)
            {
                var md = Resources.MergedDictionaries[i];
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
                Resources.MergedDictionaries.Add(rsDct);
            }
            else
            {
                // Replace the current langage dictionary with the new one
                Resources.MergedDictionaries[langDictId] = rsDct;
            }
        }

    }
}
