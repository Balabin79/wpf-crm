using System;
using System.Linq;
using Dental.Models;
using DevExpress.Xpf.Core;
using System.Windows;

using Dental.Views.Header;
using DevExpress.Mvvm.DataAnnotations;
using System.IO;
using Dental.Services;

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

        public bool CanOpenHelpForm() => true;
        public bool CanOpenRegForm() => true;
        public bool CanOpenAboutForm() => true;

        [Command]
        public void OpenHelpForm()
        {
            try
            {
                //var integration = new ContactsIntegration();
                //integration.Run();
                /*var t = db.Services.ToList();
                t.ForEach(f => f.Guid = KeyGenerator.GetUniqueKey());
                var i = db.SaveChanges();*/
                

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
