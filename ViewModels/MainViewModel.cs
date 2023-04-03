using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using B6CRM.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Navigation;

namespace B6CRM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get { return GetService<INavigationService>(); } }
        protected ISplashScreenService SplashScreenService { get { return GetService<ISplashScreenService>(); } }

        public MainViewModel() { }

        [Command]
        public void OnViewLoaded(object p)
        {
            NavigationService?.Navigate("B6CRM.Views.PatientCard.PatientsList", null, this);
            if (p is TileBarItem clientsBtn) clientsBtn.IsSelected = true;
        }

        [Command]
        public void Navigate(object p)
        {
            try
            {
                NavigationService.Navigate(p.ToString(), null, this);
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Раздел не найден!", true);
            }
        }

        [Command]
        public void Help()
        {
            try
            {
                var path = Path.Combine(new Config().PathToProgramDirectory, "B6Crm.chm");

                if (!File.Exists(path))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Не найден файл справки!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    return;
                }

                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };
                proc.Start();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке открыть файл справки!", true);
            }
        }
    }
}
