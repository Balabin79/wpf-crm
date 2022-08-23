using System;
using System.Linq;
using Dental.Models;
using DevExpress.Xpf.Core;
using System.Windows;

using Dental.Views.Header;
using DevExpress.Mvvm.DataAnnotations;
using System.IO;

namespace Dental.ViewModels
{
    class MainWindowViewModel : DevExpress.Mvvm.ViewModelBase
    {

        private readonly ApplicationContext db;
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
        public void OpenHelpForm()
        {
            try
            {
                //var integration = new ContactsIntegration();
                //integration.Run();
                

            }           
            catch (FileNotFoundException e)
            {
                //Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {

            }
        }

        [Command]
        public void OpenRegForm()
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
        public void OpenAboutForm()
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
