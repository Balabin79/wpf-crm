using System;
using System.Linq;
using Dental.Models;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using System.IO;
using Dental.Services;
using Dental.Views.About;

namespace Dental.ViewModels
{
    class HelpViewModel : DevExpress.Mvvm.ViewModelBase
    {

        private readonly ApplicationContext db;
        public HelpViewModel()
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
        public void OpenLicenseForm()
        {
            try
            {
                NewLicense = null;
                new LicenseWindow() { DataContext = this }?.ShowDialog();
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
                new InfoWindow()?.ShowDialog();
            }
            catch
            {

            }
        }

        [Command]
        public void SaveLicense()
        {
            try
            {
                
            }
            catch
            {

            }
        }

        public string License
        {
            get { return GetProperty(() => License); }
        }

        public string NewLicense
        {
            get { return GetProperty(() => NewLicense); }
            set { SetProperty(() => NewLicense, value?.Trim()); }
        }
    }
}
