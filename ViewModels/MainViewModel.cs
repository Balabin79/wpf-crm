using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using B6CRM.Models;
using B6CRM.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Navigation;
using Npgsql;

namespace B6CRM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ApplicationContext db;
        public INavigationService NavigationService { get { return GetService<INavigationService>(); } }
        protected ISplashScreenService SplashScreenService { get { return GetService<ISplashScreenService>(); } }

        public MainViewModel() => db = new ApplicationContext();

        [Command]
        public void OnViewLoaded(object p)
        {
            try
            {
                NavigationService?.Navigate("B6CRM.Views.PatientCard.PatientsList", null, this);
                if (p is TileBarItem clientsBtn) clientsBtn.IsSelected = true;
            }
            catch (Exception e)
            {
                HandleConnectError(e);
            }
        }

        [Command]
        public void Navigate(object p)
        {
            try
            {
                if (!CheckConnect())
                {
                    NavigationService?.Navigate("B6CRM.Views.FailDBConnect");
                    return;
                }

                if (!UserSessionLoaded)
                {
                    new UserSessionLoading().Run();
                    UserSessionLoaded = true;
                }

                NavigationService?.Navigate(p.ToString(), null, this);
            }
            catch (Exception e)
            {
                HandleConnectError(e);
                // Log.ErrorHandler(e, "Раздел не найден!", true);
            }
        }

        [Command]
        public void Help()
        {
            try
            {
                var path = Path.Combine(Config.defaultPath, "B6Crm.chm");

                if (!File.Exists(path))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Не найден файл справки!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    return;
                }

                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                proc.Start();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке открыть файл справки!", true);
            }
        }

        private void HandleConnectError(Exception e)
        {
            if (e is NpgsqlException || e.InnerException is NpgsqlException)
            {
                NavigationService?.Navigate("B6CRM.Views.FailDBConnect");
            }
        }

        private bool CheckConnect() 
        {
            var db = new ApplicationContext();
            // если SQLite, то пропускаем проверку, т.к. если даже отсутствует, то пересоздается локально
            if (db.Config.DbType == 0) return true;

            return Task.Run(() => { db.RolesManagment.FirstOrDefault(); }).Wait(7000); 
        }
          
        /* флаг удачной загрузки сессий */
        public bool UserSessionLoaded { get; set; } = false;
    }
}
