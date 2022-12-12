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

                // считываем значения для местоположения бд и программных файлов
                if (Config.ConnectionString == Path.Combine(Config.PathToDbDefault)) 
                { 
                    IsPathToDbDefault = true;
                    PathToDb = null;
                }
                else
                {
                    IsPathToDbDefault = false;
                    PathToDb = Config.ConnectionString;
                }

                if (Config.ConnectionString == Path.Combine(Config.PathToDbDefault))
                {
                    IsPathToProgramFilesDefault = true;
                    PathToProgramFiles = null;
                }
                else
                {
                    IsPathToProgramFilesDefault = false;
                    PathToProgramFiles = Config.PathToProgramDirectory;
                }

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Настройки\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool IsPathToDbDefault
        {
            get { return GetProperty(() => IsPathToDbDefault); }
            set { SetProperty(() => IsPathToDbDefault, value); }
        }

        public bool IsPathToProgramFilesDefault
        {
            get { return GetProperty(() => IsPathToProgramFilesDefault); }
            set { SetProperty(() => IsPathToProgramFilesDefault, value); }
        }

        public string PathToDb
        {
            get { return GetProperty(() => PathToDb); }
            set { SetProperty(() => PathToDb, value); }
        }

        public string PathToProgramFiles
        {
            get { return GetProperty(() => PathToProgramFiles); }
            set { SetProperty(() => PathToProgramFiles, value); }
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
                var pathsChanged = false;
                if (Settings?.Id == 0) db.Settings.Add(Settings);
                var employeesEdited = Employees.Where(f => db.Entry(f).State == EntityState.Modified).ToList();

                foreach (var i in employeesEdited)
                {
                    var wrapper = EmployeeWrappers.FirstOrDefault(f => f.Id == i.Id);
                    if (wrapper == null || wrapper.Password == i.Password) continue;
                    if (string.IsNullOrEmpty(i.Password)) { wrapper.Password = null; continue; }
                    i.Password = BitConverter.ToString(MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(i.Password))).Replace("-", string.Empty);
                }

                // проверяем значения
                // если галочки отмечены, то файл конфига удаляем, если сняты, то сериализуем значения в полях и создаем файл
                if (!IsPathToDbDefault || !IsPathToProgramFilesDefault)
                {
                    if (!IsPathToDbDefault)
                    {
                        if (string.IsNullOrEmpty(PathToDb?.Trim()))
                        {
                            ThemedMessageBox.Show(title: "Внимание!", text: "Поле \"Путь к базе данных\" не заполнено! Поставьте галочку \"По умолчанию\" или введите значение в поле",
                            messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                            return;
                        }
                    }

                    if (IsPathToProgramFilesDefault == false)
                    {
                        if (string.IsNullOrEmpty(PathToProgramFiles?.Trim()))
                        {
                            ThemedMessageBox.Show(title: "Внимание!", text: "Поле \"Путь к программным файлам\" не заполнено! Поставьте галочку \"По умолчанию\" или введите значение в поле",
                            messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                            return;
                        }
                    }

                    //если пути изменены по сравнению со старой версией, то 
                    pathsChanged = true;
                    var dBName = Config.defaultDBName;
                    var connectionString = PathToDb ?? Config.PathToDbDefault;
                    var pathToProgram = PathToProgramFiles ?? Config.PathToProgramDirectory;

                    var config = JsonSerializer.Serialize(new UserConfig()
                    {
                        DBName = dBName,
                        ConnectionString = connectionString,
                        PathToProgram = pathToProgram
                    });
                    File.WriteAllText("./dental.conf", config);

                    Config.ConnectionString = connectionString;
                    Config.PathToProgram = pathToProgram;

                   // var json = File.ReadAllText("./dental.conf").Trim();
                }
                /*******************/

                if (db.SaveChanges() > 0 || pathsChanged) new Notification() { Content = "Настройки сохранены!" }.run();

            }
            catch (Exception e)
            { }
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