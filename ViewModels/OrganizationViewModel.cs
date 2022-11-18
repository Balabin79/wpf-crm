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
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;

namespace Dental.ViewModels
{
    class OrganizationViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable, IImageSave
    {
        private readonly ApplicationContext db;

        public OrganizationViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Model = GetModel();
                if (Model.Id > 0) ImagesLoading();
                IsReadOnly = Model.Id > 0;

                ModelBeforeChanges = (Organization)Model.Clone();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Организации\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanSave(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OrgEditable;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OrgDeletable;


        [Command]
        public void Editable(object p) => IsReadOnly = !IsReadOnly;

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Organization Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
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


        [Command]
        public void Save(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) db.Organizations.Add(Model);

                db.SaveChanges();
                notification.Content = "Изменения сохранены в базу данных!";

                if (HasUnsavedChanges())
                {
                    notification.run();
                    ModelBeforeChanges = (Organization)Model.Clone();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
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
               // field.EditValue = null;
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
                Delete();

                Model = new Organization();

                ModelBeforeChanges = (Organization)Model.Clone();
                new Notification { Content = "Данные организации полностью удалены!" }.run();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void Delete()
        {
            try
            {
                if (db.Entry(Model).State == EntityState.Deleted) return;
                db.Entry(Model).State = EntityState.Deleted;
                db.SaveChanges();
                Logo = null;
                Stamp = null;
                new DirectoryInfo(PathToOrgDirectory).GetFiles()?.ForEach(f => f.Delete());
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }

        public Organization ModelBeforeChanges { get; set; }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model?.FieldsChanges != null) Model.FieldsChanges = new List<string>();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            string warningMessage = "\nПоля: ";
            foreach (var fieldName in Model.FieldsChanges)
            {
                warningMessage += " " + "\"" + fieldName + "\"" + ",";
            }
            warningMessage = warningMessage.Remove(warningMessage.Length - 1);

            var response = ThemedMessageBox.Show(title: "Внимание", text: "В форме организации имеются несохраненные изменения! Если вы не хотите их потерять, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой).\nИзменения:" + warningMessage,
               messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

            return response.ToString() == "Cancel";
        }

        private Organization GetModel() => db.Organizations.FirstOrDefault() ?? new Organization();

        #region Управление файлами лого, печати и подписи

        private string PathToOrgDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Organization");


        private void ImagesLoading()
        {
            try
            {
                var files = new string[] { };
                if (Directory.Exists(PathToOrgDirectory)) files = Directory.GetFiles(PathToOrgDirectory);



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
                    if (!Directory.Exists(PathToOrgDirectory)) Directory.CreateDirectory(PathToOrgDirectory);
                    var files = Directory.GetFiles(PathToOrgDirectory);
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
                        photo.CopyTo(Path.Combine(PathToOrgDirectory, "Logo" + photo.Extension), true);
                        new Notification() { Content = "Логотип сохранен!" }.run();
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
                        photo.CopyTo(Path.Combine(PathToOrgDirectory, "Stamp" + photo.Extension), true);
                        new Notification() { Content = "Файл печати сохранен!" }.run();
                    }
                }
            }
            catch (Exception e)
            {

            }
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

                    if (Directory.Exists(PathToOrgDirectory))
                    {
                        var files = Directory.GetFiles(PathToOrgDirectory);
                        foreach (var file in files) if (file.Contains(img?.Name)) File.Delete(file);
                    }
                    img.Clear();
                    msg = img?.Name == "Logo" ? "Файл логотипа удален" : "Файл печати удален";
                    new Notification() { Content = msg }.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        #endregion
    }
}
