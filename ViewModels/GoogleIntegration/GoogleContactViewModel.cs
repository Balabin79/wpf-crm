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
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using System.ComponentModel;

namespace Dental.ViewModels.GoogleIntegration
{
    public class GoogleContactViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly ApplicationContext db;
        private readonly GoogleIntegrationViewModel owner;

        public GoogleContactViewModel(GoogleIntegrationViewModel vm)
        {
            try
            {
                owner = vm;
                db = new ApplicationContext();
                Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
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
            if (p is GoogleContacts contact)
            {
                Contact = db.GoogleContacts.Include(f => f.Employee).FirstOrDefault(f => f.Id == contact.Id);
                Email = Contact.Email;
                Employee = Contact.Employee;
                CalendarName = Contact.CalendarName;
            }
            else
            {
                Contact = new GoogleContacts();
                Email = null;
                Employee = null;
                CalendarName = null;
            }
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
                    Contact.CalendarName = !string.IsNullOrEmpty(CalendarName) ? CalendarName : Employee?.FullName;
                    if (Contact?.Id == 0) 
                    { 
                        db.GoogleContacts.Add(Contact); 
                    }
                    // добавить в родительский список и там же сохранить
                }
                if (Contact?.Id == 0 && db.SaveChanges() > 0) owner.AddContact(Contact);
                if (Contact?.Id > 0 && db.SaveChanges() > 0) owner.UpdateContact();
                GoogleContactWindow?.Close();
            }
            catch(Exception e)
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
        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        [Required(ErrorMessage = @"Поле ""Email"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Email"" не более 255 символов")]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value); }
        }


        [Required(ErrorMessage = @"Поле ""Сотрудник"" обязательно для заполнения")]
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
        
        public int? IsRemoteCalendarCreated
        {
            get { return GetProperty(() => IsRemoteCalendarCreated); }
            set { SetProperty(() => IsRemoteCalendarCreated, value); }
        }

        public GoogleContactWindow GoogleContactWindow { get; set; }
        public Employee[] Employees { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

    }
}
