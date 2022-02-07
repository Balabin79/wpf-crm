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

namespace Dental.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {

        private ApplicationContext db;
        public MainWindowViewModel()
        {
            try
            {
                OpenFormHelpCommand = new LambdaCommand(OnOpenFormHelpExecuted, CanOpenFormHelpExecute);
                OpenFormRegCommand = new LambdaCommand(OnOpenFormRegExecuted, CanOpenFormRegExecute);
                OpenFormAboutCommand = new LambdaCommand(OnOpenFormAboutExecuted, CanOpenFormAboutExecute);

                db = new ApplicationContext();
               
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }


        public ICommand OpenFormHelpCommand { get; }
        public ICommand OpenFormRegCommand { get; }
        public ICommand OpenFormAboutCommand { get; }

        private bool CanOpenFormHelpExecute(object p) => true;
        private bool CanOpenFormRegExecute(object p) => true;
        private bool CanOpenFormAboutExecute(object p) => true;

        private void OnOpenFormHelpExecuted(object p)
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

        private void OnOpenFormRegExecuted(object p)
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

        private void OnOpenFormAboutExecuted(object p)
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
