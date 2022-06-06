using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using Dental.ViewModels.GoogleIntegration;
using Dental.Views.Integration.Google;

namespace Dental.ViewModels.GoogleIntegration
{
    public class GoogleContactViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public GoogleContactViewModel()
        {
            try
            {
                db = new ApplicationContext();              
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с модулем \"Google - контакты\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenContactForm(object p)
        {
            Contact = int.TryParse(p?.ToString(), out int param) ? db.GoogleContacts.Include(f => f.Employee).FirstOrDefault(f => f.Id == param) : new GoogleContacts();
            GoogleContactWindow = new GoogleContactWindow() { DataContext = this };
            GoogleContactWindow.Show();
        }

        [Command]
        public void CloseContactForm() => GoogleContactWindow?.Close();


        [Command]
        public void ContactSave()
        {
            try
            {
                if (Contact?.Email != Email || Contact?.Employee?.Guid != Employee?.Guid || Contact?.CalendarName != CalendarName)
                {
                    Contact.Email = Email;
                    Contact.Employee = Employee;
                    Contact.CalendarName = CalendarName;
                    //if (Contact?.Id == 0) db.Settings.Add(Settings);
                }
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                //IsEnabled = true;
               // GoogleAccountWindow?.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранить данные в бд возникла ошибка!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ContactDelete()
        {

        }

        //Модель 
        public GoogleContacts Contact
        {
            get { return GetProperty(() => Contact); }
            set { SetProperty(() => Contact, value); }
        }

        //Свойства
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value); }
        }

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }

        public string CalendarName
        {
            get { return GetProperty(() => CalendarName); }
            set { SetProperty(() => CalendarName, value); }
        }

        public GoogleContactWindow GoogleContactWindow { get; set; }

    }
}
