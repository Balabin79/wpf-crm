using System;
using System.Linq;
using Dental.Models;
using System.Data.Entity;
using System.Collections;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Services;
using System.Collections.Generic;
using Dental.Infrastructures.Logs;
using Dental.Views.EmployeeDir;
using Dental.Views.Header;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class MainWindowViewModel : DevExpress.Mvvm.ViewModelBase
    {

        private ApplicationContext db;
        public MainWindowViewModel()
        {
            try
            {
                db = new ApplicationContext();              
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenHelpForm(object p)
        {
            try
            {
                HelpWin = new HelpWindow();
                HelpWin.ShowDialog();
            }
            catch
            {

            }
        }

        [Command]
        public void OpenRegForm(object p)
        {
            try
            {
                RegWin = new RegWindow();
                RegWin.ShowDialog();
            }
            catch
            {

            }
        }

        [Command]
        public void OpenAboutForm(object p)
        {
            try
            {
                AboutWin = new AboutWindow();
                AboutWin.ShowDialog();
            }
            catch
            {

            }
        }

        public HelpWindow HelpWin { get; set; }
        public RegWindow RegWin { get; set; }
        public AboutWindow AboutWin { get; set; }

    }
}
