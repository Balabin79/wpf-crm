using System;
using System.Text.Json;
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
using System.Collections.Generic;
using Dental.Models.Settings;

namespace Dental.ViewModels
{
    public class SettingsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public SettingsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Settings = db.Settings.FirstOrDefault() ?? new Setting();
                IsReadOnly = true;
                Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection();
                Roles = db.RolesManagment.OrderBy(f => f.Num).ToArray();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Настройки\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanEditable() => ((UserSession)Application.Current.Resources["UserSession"]).SettingsRead;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).SettingsRead;

        public Setting Settings
        {
            get { return GetProperty(() => Settings); }
            set { SetProperty(() => Settings, value); }
        }

        public ICollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Account Account
        {
            get { return GetProperty(() => Account); }
            set { SetProperty(() => Account, value); }
        }

        public RolesEnabled RolesEnabled
        {
            get { return GetProperty(() => RolesEnabled); }
            set { SetProperty(() => RolesEnabled, value); }
        }

        public ICollection<RoleManagment> Roles { get; set; }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        [Command]
        public void Save()
        {
            try
            {
                if (Settings?.Id == 0) db.Settings.Add(Settings);
                if (db.SaveChanges() > 0) new Notification() { Content = "Настройки сохранены!" }.run();
            }
            catch (Exception e)
            {
            }
        }
    }


}