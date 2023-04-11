using System;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.Native;
using B6CRM.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using B6CRM.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;
using System.Collections.Generic;
using B6CRM.Models.Settings;
using B6CRM.Views.About;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;
using B6CRM.Views.Settings;
using System.Windows.Media.Imaging;
using B6CRM.ViewModels.Org;
using System.Windows.Media;
using License;
using System.Text.RegularExpressions;
using B6CRM.Models;
using B6CRM.Infrastructures.Extensions;
using B6CRM.Models.Base;
using B6CRM.Services;
using B6CRM.Views.WindowForms;
using Npgsql;

namespace B6CRM.ViewModels
{
    public class SettingsViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {
        private ApplicationContext db;

        public SettingsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Config = new Config();
                SettingsVM = new SettingsVM();

                var model = db.Settings.FirstOrDefault() ?? new Setting();
                SettingsVM.Copy(model);
                //Roles = db.RolesManagment.OrderBy(f => f.Num).ToArray();

                IsReadOnly = model?.Id > 0;
                if (IsReadOnly) ImagesLoading();

                Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection();

                EmployeeWrappers = new List<EmployeeWrapper>();
                Employees.ForEach(f => EmployeeWrappers.Add(new EmployeeWrapper { Id = f.Id, Password = f.Password }));

                Roles = db.RolesManagment.OrderBy(f => f.Num).ToArray();

                NotificationEvents = db.NotificationEvents?.Include(f => f.TelegramBot)?.ToArray();

                SetTelegramBots();
            }           
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public ICollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }

        public ICollection<EmployeeWrapper> EmployeeWrappers { get; set; }

        public ICollection<RoleManagment> Roles { get; set; }

        public ICollection<NotificationEvent> NotificationEvents { get; set; }

        public SettingsVM SettingsVM
        {
            get { return GetProperty(() => SettingsVM); }
            set { SetProperty(() => SettingsVM, value); }
        }        

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        [Command]
        public void Save()
        {
            try
            {
                var model = db.Settings.FirstOrDefault() ?? new Setting();

                model.OrgName = SettingsVM.OrgName;
                model.OrgShortName = SettingsVM.OrgShortName;
                model.OrgAddress = SettingsVM.OrgAddress;
                model.OrgPhone = SettingsVM.OrgPhone;
                model.OrgEmail = SettingsVM.OrgEmail;
                model.OrgSite = SettingsVM.OrgSite;
                model.IsPasswordRequired = SettingsVM.IsPasswordRequired;
                model.RolesEnabled = SettingsVM.RolesEnabled;
                if (model?.Id == 0) db.Settings.Add(model);

                #region лицензия 
                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && Status.Evaluation_Time_Current > Status.Evaluation_Time)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                #endregion


                var employeesEdited = Employees.Where(f => db.Entry(f).State == EntityState.Modified).ToList();
                foreach (var i in employeesEdited)
                {
                    var wrapper = EmployeeWrappers.FirstOrDefault(f => f.Id == i.Id);
                    if (wrapper == null || wrapper.Password == i.Password) continue;
                    if (string.IsNullOrEmpty(i.Password)) { wrapper.Password = null; continue; }
                    i.Password = BitConverter.ToString(MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(i.Password))).Replace("-", string.Empty);
                }

                if (db.SaveChanges() > 0)
                {
                    SettingsVM.Id = model.Id;
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при сохранении настроек!", true);
            }
        }

        #region Свойства
        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public ImageSource Logo
        {
            get { return GetProperty(() => Logo); }
            set { SetProperty(() => Logo, value); }
        }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

        #endregion

        #region Управление файлами лого

        public void ImagesLoading()
        {
            try
            {
                var files = new string[] { };
                if (Directory.Exists(Config.PathToOrgDirectory)) files = Directory.GetFiles(Config.PathToOrgDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    using (var stream = new FileStream(files[i], FileMode.Open))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = stream;
                        img.EndInit();
                        img.Freeze();

                        if (files[i].Contains("Logo")) Logo = img;
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void ImageSave(object p)
        {
            try
            {
                if (p is ImageEditEx img && img.HasImage)
                {
                    if (!Directory.Exists(Config.PathToOrgDirectory)) Directory.CreateDirectory(Config.PathToOrgDirectory);
                    var files = Directory.GetFiles(Config.PathToOrgDirectory);
                    if (img.Name == "Logo")
                    {
                        foreach (var file in files)
                        {
                            if (file.Contains("Logo"))
                            {
                                var response = ThemedMessageBox.Show(title: "Вы уверены?", text: "Заменить файл логотипа?",
                                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                                if (response.ToString() == "No") return;
                                File.SetAttributes(file, FileAttributes.Normal);
                                File.Delete(file);
                                break;
                            }
                        }
                        FileInfo photo = new FileInfo(img.ImagePath);
                        string fileFullName = Path.Combine(Config.PathToOrgDirectory, "Logo" + photo.Extension);
                        photo.CopyTo(fileFullName, true);
                        File.SetAttributes(fileFullName, FileAttributes.Normal);
                        new Notification() { Content = "Логотип сохранен!" }.run();
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void ImageDelete(object p)
        {
            try
            {
                //string msg = "";
                if (p is ImageEditEx img)
                {
                    //if (img?.Name == "Logo") msg = "Удалить файл логотипа?";
                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить файл логотипа?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    if (Directory.Exists(Config.PathToOrgDirectory))
                    {
                        var files = Directory.GetFiles(Config.PathToOrgDirectory);
                        foreach (var file in files) if (file.Contains(img?.Name))
                            {
                                File.SetAttributes(file, FileAttributes.Normal);
                                File.Delete(file);
                            }
                    }
                    img.Clear();
                    // msg = img?.Name == "Logo" ? "Файл логотипа удален" : "Файл печати удален";
                    db.SaveChanges();
                    new Notification() { Content = "Файл логотипа удален" }.run();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        #endregion

        #region License
        [Command]
        public void OpenLicenseForm()
        {
            try
            {
                new LicenseWindow() { DataContext = new LicViewModel() }?.ShowDialog();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void OpenAboutForm()
        {
            try
            {
                new InfoWindow()?.ShowDialog();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
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
        #endregion

        #region TelegramBots
        public ObservableCollection<TelegramBot> TelegramBots
        {
            get { return GetProperty(() => TelegramBots); }
            set { SetProperty(() => TelegramBots, value); }
        }

        [Command]
        public void OpenTelegramBotsWindow()
        {
            try
            {
                db.TelegramBots.ForEach(f => db.Entry(f).State = EntityState.Unchanged);

                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == TelegramBotsWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }
                TelegramBotsWindow = new TelegramBotsWindow() { DataContext = this };
                TelegramBotsWindow.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void AddTelegramBot()
        {
            try
            {
                TelegramBots.Add(new TelegramBot());
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeleteTelegramBot(object p)
        {
            try
            {
                if (p is TelegramBot model)
                {
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                    }
                    else db.Entry(model).State = EntityState.Detached;
                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Телеграм-бот удален из базы данных!" }.run();
                        SetTelegramBots();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void SaveTelegramBot()
        {
            try
            {
                foreach (var item in TelegramBots)
                {
                    if (string.IsNullOrEmpty(item.Token)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                if (db.SaveChanges() > 0)
                {
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    SetTelegramBots();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public void SetTelegramBots() => TelegramBots = db.TelegramBots.ToObservableCollection();

        public TelegramBotsWindow TelegramBotsWindow { get; set; }
        #endregion
    }

    public class EmployeeWrapper
    {
        public int? Id { get; set; }
        public string Password { get; set; }
    }
}