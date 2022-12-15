using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Extensions;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using Dental.ViewModels.Org;
using Dental.Views.Settings;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;

namespace Dental.ViewModels.Org
{
    public class OrganizationViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {

        private readonly ApplicationContext db;
        public OrganizationViewModel()
        {
            try
            {
                db = new ApplicationContext();
                OrganizationVM = new OrganizationVM();
                var model = db.Organizations.FirstOrDefault() ?? new Organization();
                OrganizationVM.Copy(model);
                IsReadOnly = model?.Id > 0;
                if (IsReadOnly) ImagesLoading();
            }
            catch (Exception e)
            {
                var response = ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Организации\"! Проверьте настройки подключения к базе данных.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                if (response.ToString() == "OK")
                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
        }

        public bool CanSave(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OrgEditable;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OrgDeletable;

        [Command]
        public void Editable(object p) => IsReadOnly = !IsReadOnly;

        [Command]
        public void Save(object p)
        {
            try
            {
                var model = db.Organizations.FirstOrDefault() ?? new Organization();

                model.Name = OrganizationVM.Name;
                model.ShortName = OrganizationVM.ShortName;
                model.Kpp = OrganizationVM.Kpp;
                model.Inn = OrganizationVM.Inn;
                model.Address = OrganizationVM.Address;
                model.Phone = OrganizationVM.Phone;
                model.Email = OrganizationVM.Email;
                model.AccountNumber = OrganizationVM.AccountNumber;
                model.CorrAccountNumber = OrganizationVM.CorrAccountNumber;
                model.Bik = OrganizationVM.Bik;
                model.BankName = OrganizationVM.BankName;
                model.Ogrn = OrganizationVM.Ogrn;
                model.LicenseDate = OrganizationVM.LicenseDate;
                model.GeneralDirector = OrganizationVM.GeneralDirector;
                model.LicenseName = OrganizationVM.LicenseName;
                model.Site = OrganizationVM.Site;
                model.WhoIssuedBy = OrganizationVM.WhoIssuedBy;
                model.Okpo = OrganizationVM.Okpo;

                if (model?.Id == 0) db.Organizations.Add(model);
                if (db.SaveChanges() > 0)
                {
                    OrganizationVM.Id = model.Id;
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }

            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при сохранении данных организации!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ClearDate(object p)
        {
            if (p is DateEdit field)
            {
                field.ClearError();
                field.Clear();
                field.ClosePopup();
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Вы уверены, что хотите полностью удалить данные организации?",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                var model = db.Organizations.FirstOrDefault();
                if (model == null || db.Entry(model).State == EntityState.Deleted) return;

                db.Entry(model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0)
                {
                    OrganizationVM = new OrganizationVM();
                    Logo = null;
                    Stamp = null;
                    if (Directory.Exists(Config.PathToOrgDirectory)) new DirectoryInfo(Config.PathToOrgDirectory)?.GetFiles()?.ForEach(f => f.Delete());

                    new Notification { Content = "Данные организации полностью удалены!" }.run();
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        #region Управление файлами лого и печати

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
                        if (files[i].Contains("Stamp")) Stamp = img;
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
                                File.Delete(file);
                                break;
                            }
                        }
                        FileInfo photo = new FileInfo(img.ImagePath);
                        photo.CopyTo(Path.Combine(Config.PathToOrgDirectory, "Logo" + photo.Extension), true);
                        new Notification() { Content = "Логотип сохранен!" }.run();
                        db.SaveChanges();
                    }

                    if (img.Name == "Stamp")
                    {
                        foreach (var file in files)
                        {
                            if (file.Contains("Stamp"))
                            {
                                var response = ThemedMessageBox.Show(title: "Вы уверены?", text: "Заменить файл печати?",
                                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                                if (response.ToString() == "No") return;
                                File.Delete(file);
                                break;
                            }
                        }
                        FileInfo photo = new FileInfo(img.ImagePath);
                        photo.CopyTo(Path.Combine(Config.PathToOrgDirectory, "Stamp" + photo.Extension), true);
                        new Notification() { Content = "Файл печати сохранен!" }.run();
                    }
                }
            }
            catch { }
        }

        [Command]
        public void ImageDelete(object p)
        {
            try
            {
                string msg = "";
                if (p is ImageEditEx img)
                {
                    if (img?.Name == "Logo") msg = "Удалить файл логотипа?";
                    if (img?.Name == "Stamp") msg = "Удалить файл печати?";

                    var response = ThemedMessageBox.Show(title: "Внимание", text: msg, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    if (Directory.Exists(Config.PathToOrgDirectory))
                    {
                        var files = Directory.GetFiles(Config.PathToOrgDirectory);
                        foreach (var file in files) if (file.Contains(img?.Name)) File.Delete(file);
                    }
                    img.Clear();
                    msg = img?.Name == "Logo" ? "Файл логотипа удален" : "Файл печати удален";
                    db.SaveChanges();
                    new Notification() { Content = msg }.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        #endregion

        #region Свойства
        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public OrganizationVM OrganizationVM
        {
            get { return GetProperty(() => OrganizationVM); }
            set { SetProperty(() => OrganizationVM, value); }
        }

        public ImageSource Logo
        {
            get { return GetProperty(() => Logo); }
            set { SetProperty(() => Logo, value); }
        }

        public ImageSource Stamp
        {
            get { return GetProperty(() => Stamp); }
            set { SetProperty(() => Stamp, value); }
        }

        #endregion
    }
}
