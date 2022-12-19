using Dental.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Windows;

namespace Dental
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
         App()
         {
            try
            {
                ApplicationThemeHelper.ApplicationThemeName = Theme.Office2019WhiteName;
                SplashScreenManager.CreateFluent(
                    new DXSplashScreenViewModel
                    {
                        Copyright = "Все права защищены",
                        IsIndeterminate = true,
                        Logo = null,//new System.Uri("pack://application:,,,/Resources/Icons/zzz.png", UriKind.RelativeOrAbsolute),
                        Status = "Запуск...",
                        Title = "B6 Dental",
                        Subtitle = "Автоматизация стоматологической клиники"
                    }
                    ).ShowOnStartup();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message +  ex.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message  + e.InnerException.Message);
            }

         }
    }


}
