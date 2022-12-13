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
using Dental.Views.About;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Dental.Views.Settings;

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
                Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection();
                Roles = db.RolesManagment.OrderBy(f => f.Num).ToArray();

                IsReadOnly = true;

                EmployeeWrappers = new List<EmployeeWrapper>();
                Employees.ForEach(f => EmployeeWrappers.Add(new EmployeeWrapper { Id = f.Id, Password = f.Password }));
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

        public ICollection<EmployeeWrapper> EmployeeWrappers { get; set; }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
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
                var employeesEdited = Employees.Where(f => db.Entry(f).State == EntityState.Modified).ToList();

                foreach (var i in employeesEdited)
                {
                    var wrapper = EmployeeWrappers.FirstOrDefault(f => f.Id == i.Id);
                    if (wrapper == null || wrapper.Password == i.Password) continue;
                    if (string.IsNullOrEmpty(i.Password)) { wrapper.Password = null; continue; }
                    i.Password = BitConverter.ToString(MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(i.Password))).Replace("-", string.Empty);
                }

                if (db.SaveChanges() > 0) new Notification() { Content = "Настройки сохранены!" }.run();
            }
            catch (Exception e)
            { }
        }

        [Command]
        public void OpenPathsSettingsForm()
        {
            try
            {
                new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
            catch { }
        }


        [Command]
        public void OpenLicenseForm()
        {
            try
            {
                NewLicense = null;
                new LicenseWindow() { DataContext = this }?.ShowDialog();
            }
            catch { }
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

    public class EmployeeWrapper
    {
        public int? Id { get; set; }
        public string Password { get; set; }
    }

}