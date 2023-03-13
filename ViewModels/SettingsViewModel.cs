using System;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Microsoft.EntityFrameworkCore;
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
using Dental.Infrastructures.Extensions;
using System.Windows.Media.Imaging;
using Dental.ViewModels.Org;
using System.Windows.Media;
using License;
using Dental.Models.Base;
using System.Text.RegularExpressions;

namespace Dental.ViewModels
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

                if (File.Exists(Config.PathToConfig))
                {
                    var json = File.ReadAllText(Config.PathToConfig).Trim();
                    if (json.Length > 10 && JsonSerializer.Deserialize(json, new StoreConnectToDb().GetType()) is StoreConnectToDb config)
                    {
                        SettingsVM.DbType = config.Db;
                        SettingsVM.PostgresConnect = config.PostgresConnect;
                    }
                }

                if (SettingsVM.PostgresConnect == null) SettingsVM.PostgresConnect = new PostgresConnect();

                var model = db.Settings.FirstOrDefault() ?? new Setting();                        
                SettingsVM.Copy(model);          
                //Roles = db.RolesManagment.OrderBy(f => f.Num).ToArray();

                IsReadOnly = model?.Id > 0;
                if (IsReadOnly) ImagesLoading();
            }
            catch (Exception e)
            {
                /*ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Настройки\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);*/
            }
        }

        public SettingsVM SettingsVM
        {
            get { return GetProperty(() => SettingsVM); }
            set { SetProperty(() => SettingsVM, value); }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        private bool IsConfigChanged { get; set; } = false;

        [Command]
        public void SaveConfig()
        {
            // возможно изменили настройки подключения, пробуем коннектиться, если сбой, то выходим
            Setting model;
            StoreConnectToDb connect = new StoreConnectToDb();
            try
            {
                // пытаемся применить новые параметры подключения
                if (SettingsVM.DbType == null || SettingsVM.DbType == 0)
                {
                    Config.DbType = 0;
                    Config.ConnectionString = Config.PathToDbDefault;
                    SettingsVM.PostgresConnect = null;
                }
                else
                {
                    Config.DbType = 1;
                    Config.ConnectionString = $"Host={SettingsVM.PostgresConnect?.Host ?? ""};Port={SettingsVM.PostgresConnect?.Port};Database={SettingsVM.PostgresConnect?.Database ?? ""};Username={SettingsVM.PostgresConnect?.Username ?? ""};Password={SettingsVM.PostgresConnect?.Password ?? ""};";
                }
                db = new ApplicationContext();
                db.Config = Config;

                // eсли подключение не упало, то сохраняем настройки подключения
                model = db.Settings.FirstOrDefault() ?? new Setting();               
                ConfigHandler();

                if (IsConfigChanged) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();

                IsConfigChanged = false;
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Сбой при подключении к базе данных, попробуйте изменить параметры подключения!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                return;
            }
        }

        private void ConfigHandler()
        {
            // сравниваем значения из формы с теми что из файла, если изменились, то изменяем флаг оповещения
            if (File.Exists(Config.PathToConfig))
            {
                var json = File.ReadAllText(Config.PathToConfig).Trim();

                if (json.Length > 10 && JsonSerializer.Deserialize(json, new StoreConnectToDb().GetType()) is StoreConnectToDb file)
                {
                    // значения не поменялись
                    if (
                        SettingsVM.DbType == file.Db
                        && Config.ConnectionString == file.ConnectionString
                        && SettingsVM.PostgresConnect?.Database == file.PostgresConnect?.Database
                        && SettingsVM.PostgresConnect?.Password == file.PostgresConnect?.Password
                        && SettingsVM.PostgresConnect?.Username == file.PostgresConnect?.Username
                        && SettingsVM.PostgresConnect?.Port == file.PostgresConnect?.Port
                        && SettingsVM.PostgresConnect?.Host == file.PostgresConnect?.Host
                        ) return;
                }
            }

            var connect = new StoreConnectToDb();
            if (SettingsVM.DbType == 0)
            {
                connect.Db = 0;
                connect.ConnectionString = Config.PathToDbDefault;
                SettingsVM.PostgresConnect = null;
            }

            if (SettingsVM.DbType == 1)
            {
                connect.Db = 1;
                connect.ConnectionString = $"Host={SettingsVM.PostgresConnect?.Host ?? ""};Port={SettingsVM.PostgresConnect?.Port};Database={SettingsVM.PostgresConnect?.Database ?? ""};Username={SettingsVM.PostgresConnect?.Username ?? ""};Password={SettingsVM.PostgresConnect?.Password ?? ""};";

                connect.PostgresConnect = SettingsVM.PostgresConnect;
            }

            var config = JsonSerializer.Serialize(connect);

            if (File.Exists(Config.PathToConfig)) File.Delete(Config.PathToConfig);
            File.WriteAllText(Config.PathToConfig, config);

            Config.ConnectionString = connect.ConnectionString ?? Config.PathToDbDefault;
            Config.DbType = connect.Db;
            IsConfigChanged = true;
        }

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
                if (model?.Id == 0) db.Settings.Add(model);              

                #region лицензия 
                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                #endregion

                if (db.SaveChanges() > 0)
                {
                    SettingsVM.Id = model.Id;
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }           
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при сохранении настроек!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                (new ViewModelLog(e)).run();
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
            { }
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
                (new ViewModelLog(e)).run();
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
            { }
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
        #endregion

    }

}