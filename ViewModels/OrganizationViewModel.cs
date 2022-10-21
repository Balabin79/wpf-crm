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
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class OrganizationViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable
    {
        private readonly ApplicationContext db;

        public OrganizationViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Files = new ObservableCollection<FileInfo>();
                Model = GetModel();
                if (Model.Id > 0) ImagesLoading();
                IsReadOnly = Model.Id > 0;

                Files = Directory.Exists(PathToOrgFilesDirectory) ? 
                    new DirectoryInfo(PathToOrgFilesDirectory).GetFiles().ToObservableCollection() : 
                    new ObservableCollection<FileInfo>();
                
                ModelBeforeChanges = (Organization)Model.Clone();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Организации\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

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

        public ImageSource Signature
        {
            get { return GetProperty(() => Signature); }
            set { SetProperty(() => Signature, value); }
        }

        public ObservableCollection<FileInfo> Files
        {
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }

        [Command]
        public void Save(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) db.Organizations.Add(Model);
                SaveLogo();
                SaveStamp();
                SaveSignature();
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
                new Notification {Content = "Данные организации полностью удалены!" }.run();
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
                Signature = null;
                Files.Clear();
                new DirectoryInfo(PathToOrgFilesDirectory).GetFiles()?.ForEach(f => f.Delete());
                new DirectoryInfo(PathToLogoDirectory).GetFiles()?.ForEach(f => f.Delete());
                new DirectoryInfo(PathToStampDirectory).GetFiles()?.ForEach(f => f.Delete());
                new DirectoryInfo(PathToSignatureDirectory).GetFiles()?.ForEach(f => f.Delete());
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
        private string PathToOrgFilesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Organization", "Files");
        private string PathToLogoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Organization", "Logo");
        private string PathToStampDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Organization", "Stamp");
        private string PathToSignatureDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Organization", "Signature");

        private void ImagesLoading()
        {
            try
            {
                if (Directory.Exists(PathToLogoDirectory))
                {
                    var files = Directory.GetFiles(PathToLogoDirectory);
                    if (files.Length > 0) Model.LogoFilePath = files[0];
                }
                if (Directory.Exists(PathToStampDirectory))
                {
                    var files = Directory.GetFiles(PathToStampDirectory);
                    if (files.Length > 0) Model.StampFilePath = files[0];
                }
                if (Directory.Exists(PathToSignatureDirectory))
                {
                    var files = Directory.GetFiles(PathToSignatureDirectory);
                    if (files.Length > 0) Model.SignatureFilePath = files[0];
                }

                string[] images = new string[] { Model.LogoFilePath, Model.StampFilePath, Model.SignatureFilePath };
                for (int i = 0; i < images.Length; i++)
                {

                        if (!string.IsNullOrEmpty(images[i]) && File.Exists(images[i]))
                    {
                        using (var stream = new FileStream(images[i], FileMode.Open))
                        {
                            var img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.StreamSource = stream;
                            img.EndInit();
                            img.Freeze();
                            switch (i)
                            {
                                case 0: Logo = img; break;
                                case 1: Stamp = img; break;
                                case 2: Signature = img; break;
                            }
                        }
                    }
                    else images[i] = null;
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveLogo()
        {
            try
            {
                if (Logo == null || (Model?.LogoFilePath!=null && ModelBeforeChanges?.LogoFilePath !=null && Model?.LogoFilePath == ModelBeforeChanges?.LogoFilePath)) return;
                if (Logo is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.LogoFilePath) return;
                }

                if (!string.IsNullOrEmpty(Model?.LogoFilePath))
                {
                    if (!Directory.Exists(PathToLogoDirectory)) Directory.CreateDirectory(PathToLogoDirectory);
                    FileInfo logo = new FileInfo(Model?.LogoFilePath);
                    if (!logo.Exists) logo.Create();
                    logo.CopyTo(Path.Combine(PathToLogoDirectory, logo.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(PathToLogoDirectory, logo.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.LogoFilePath = newFile.FullName;

                    // подчищаем директорию. Оставляем только файл, который используется в качестве логотипа, остальные удаляем.
                    var files = new DirectoryInfo(PathToLogoDirectory).GetFiles();
                    foreach (var file in files) if (file.FullName != newFile.FullName) file.Delete();
                    //LogoLoading();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveStamp()
        {
            try
            {
                if (Stamp == null || (Model?.StampFilePath != null && ModelBeforeChanges?.StampFilePath != null && Model?.StampFilePath == ModelBeforeChanges?.StampFilePath)) return;
                if (Logo is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.StampFilePath) return;
                }

                if (!string.IsNullOrEmpty(Model?.StampFilePath))
                {
                    if (!Directory.Exists(PathToStampDirectory)) Directory.CreateDirectory(PathToStampDirectory);
                    FileInfo logo = new FileInfo(Model?.StampFilePath);
                    if (!logo.Exists) logo.Create();
                    logo.CopyTo(Path.Combine(PathToStampDirectory, logo.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(PathToStampDirectory, logo.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.StampFilePath = newFile.FullName;

                    var files = new DirectoryInfo(PathToStampDirectory).GetFiles();
                    foreach (var file in files) if (file.FullName != newFile.FullName) file.Delete();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveSignature()
        {
            try
            {
                if (Signature == null || (Model?.SignatureFilePath != null && ModelBeforeChanges?.SignatureFilePath != null && Model?.SignatureFilePath == ModelBeforeChanges?.SignatureFilePath)) return;
                if (Logo is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.SignatureFilePath) return;
                }

                if (!string.IsNullOrEmpty(Model?.SignatureFilePath))
                {
                    if (!Directory.Exists(PathToSignatureDirectory)) Directory.CreateDirectory(PathToSignatureDirectory);
                    FileInfo logo = new FileInfo(Model?.SignatureFilePath);
                    if (!logo.Exists) logo.Create();
                    logo.CopyTo(Path.Combine(PathToSignatureDirectory, logo.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(PathToSignatureDirectory, logo.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.SignatureFilePath = newFile.FullName;

                    var files = new DirectoryInfo(PathToSignatureDirectory).GetFiles();
                    foreach (var file in files) if (file.FullName != newFile.FullName) file.Delete();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void ImageDelete(object p)
        {
            try
            {
                string path = "";
                string msg = "";
                if (p is FrameworkElement name)
                {
                    if (name?.Name == "Logo")
                    {
                        msg = "Удалить файл логотипа?";
                        path = PathToLogoDirectory;
                    }
                    if (name?.Name == "Stamp")
                    {
                        msg = "Удалить файл печати?";
                        path = PathToStampDirectory;
                    }
                    if (name?.Name == "Signature")
                    {
                        msg = "Удалить файл подписи?";
                        path = PathToSignatureDirectory;
                    }
                    if (string.IsNullOrEmpty(path)) return;
                }


                var response = ThemedMessageBox.Show(title: "Внимание", text: msg, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;
                if (Directory.Exists(path))
                {
                    new DirectoryInfo(path).GetFiles()?.ForEach(f => f.Delete());
                }

                if (p is Infrastructures.Extensions.ImageEditEx ie) ie.Clear();
                ImagesLoading();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        #endregion

        #region команды, связанных с прикреплением файлов 
        [Command]
        public void OpenDirectory(object p)
        {
            try
            {
                if (Directory.Exists(PathToOrgFilesDirectory)) Process.Start(PathToOrgFilesDirectory);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую файл директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void ExecuteFile(object p)
        {
            try
            {
                if (p is FileInfo file) Process.Start(file.FullName);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно выполнить загрузку файла!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void AttachmentFile(object p)
        {
            try
            {
                var filePath = string.Empty;
                using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    dialog.InitialDirectory = "c:\\";
                    dialog.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    dialog.FilterIndex = 2;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    filePath = dialog.FileName;
                    if (string.IsNullOrEmpty(filePath)) return;
                }

                FileInfo file = new FileInfo(filePath);

                // проверяем на наличие существующего файла
                foreach (var i in Files)
                {
                    if (string.Compare(i.Name, file.Name, StringComparison.CurrentCulture) == 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Файл с таким именем уже есть в списке прикрепленных файлов. Заменить текущий файл?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return; // не захотел, поэтому дальше ничего не делаем

                        // Решил заменить файл, удаляем файл, добавляем новый и перезагружаем коллекцию
                        i.Delete();
                    }
                }

                if (!Directory.Exists(PathToOrgFilesDirectory)) Directory.CreateDirectory(PathToOrgFilesDirectory);

                File.Copy(file.FullName, Path.Combine(PathToOrgFilesDirectory, file.Name), true);

                FileInfo newFile = new FileInfo(Path.Combine(PathToOrgFilesDirectory, file.Name));
                newFile.CreationTime = DateTime.Now;

                Files = new DirectoryInfo(PathToOrgFilesDirectory).GetFiles().ToObservableCollection();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void DeleteFile(object p)
        {
            try
            {
                if (p is FileInfo file)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить файл с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    file.Delete();
                    Files = new DirectoryInfo(PathToOrgFilesDirectory).GetFiles().ToObservableCollection();
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
