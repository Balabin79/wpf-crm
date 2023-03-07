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
using Dental.Infrastructures.Extensions;
using System.Windows.Media.Imaging;
using Dental.ViewModels.Org;
using System.Windows.Media;
using License;
using Dental.Models.Base;

namespace Dental.ViewModels
{
    public class SettingsViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {
        private readonly ApplicationContext db;

        public SettingsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                var model = db.Settings.FirstOrDefault() ?? new Setting();
                Config = new Config();
                SettingsVM = new SettingsVM();
                SettingsVM.Copy(model);

                //Roles = db.RolesManagment.OrderBy(f => f.Num).ToArray();

                IsReadOnly = model?.Id > 0;
                if (IsReadOnly) ImagesLoading();

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Настройки\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public PostgresConnect PostgresConnect
        {
            get { return GetProperty(() => PostgresConnect); }
            set { SetProperty(() => PostgresConnect, value); }
        }

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

            if (model?.Id == 0) db.Settings.Add(model);

            /************************/
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
            /************************/

            if (db.SaveChanges() > 0)
            {
                    SettingsVM.Id = model.Id;
                new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }

        }
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при сохранении данных организации!",
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