using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dental.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Navigation;

namespace Dental.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public INavigationService NavigationService { get { return this.GetService<INavigationService>(); } }

        public MainViewModel() { }

        [Command]
        public void OnViewLoaded(object p)
        {
            NavigationService.Navigate("Dental.Views.PatientCard.PatientsList", null, this);
            if (p is TileBarItem clientsBtn) clientsBtn.IsSelected = true;
        }

        [Command]
        public void Navigate(object p)
        {
            try
            {
                NavigationService.Navigate(p.ToString(), null, this);
            }
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Раздел не найден!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Help()
        {
            try
            {
                var path = Path.Combine(new Config().PathToProgramDirectory, "B6Dental.chm");
                if (!File.Exists(path))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Не найден файл справки!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    return;
                }
                Process.Start(path);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть файл справки!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

    }
}
