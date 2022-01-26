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
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class OrganizationViewModel : ViewModelBase, IImageDeletable
    {
        private readonly ApplicationContext db;
       
        public OrganizationViewModel()
        {
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);

            #region инициализация команд, связанных с прикреплением к карте пациента файлов
            DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
            ExecuteFileCommand = new LambdaCommand(OnExecuteFileCommandExecuted, CanExecuteFileCommandExecute);
            OpenDirectoryCommand = new LambdaCommand(OnOpenDirectoryCommandExecuted, CanOpenDirectoryCommandExecute);
            AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
            #endregion

            IsReadOnly = true;
            try
            {
                db = new ApplicationContext();
                Files = new ObservableCollection<FileInfo>();
                Model = GetModel();
                ImagesLoading();
                if (Model != null)
                {
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
                    if (Directory.Exists(PathToOrgDirectory))
                    {
                        Files = new DirectoryInfo(PathToOrgDirectory).GetFiles().ToObservableCollection();
                    }
                } 
                else
                {
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                ModelBeforeChanges = (Organization)Model.Clone();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Организации\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
            Navigator.HasUnsavedChanges = HasUnsavedChanges;
            Navigator.UserSelectedBtnCancel = UserSelectedBtnCancel;
        }

        #region Блокировка полей
        public ICommand EditableCommand { get; }
        private bool CanEditableCommandExecute(object p) => true;
        private void OnEditableCommandExecuted(object p)
        {
            try
            {
                IsReadOnly = !IsReadOnly;
                BtnIconEditableHide = IsReadOnly;
                BtnIconEditableVisible = !IsReadOnly;
                if (Model != null && Model.Id != 0) BtnAfterSaveEnable = !IsReadOnly;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool _BtnIconEditableVisible;
        public bool BtnIconEditableVisible
        {
            get => _BtnIconEditableVisible;
            set => Set(ref _BtnIconEditableVisible, value);
        }

        private bool _BtnIconEditableHide;
        public bool BtnIconEditableHide
        {
            get => _BtnIconEditableHide;
            set => Set(ref _BtnIconEditableHide, value);
        }

        public bool _BtnAfterSaveEnable = false;
        public bool BtnAfterSaveEnable
        {
            get => _BtnAfterSaveEnable;
            set => Set(ref _BtnAfterSaveEnable, value);
        }

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }
        #endregion

        #region Управление моделью
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanSaveCommandExecute(object p) => true;
        private bool CanDeleteCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
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

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Вы уверены, что хотите полностью удалить данные организации?",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;
                Delete();

                Model = new Organization();
                var notification = new Notification();
                ModelBeforeChanges = (Organization)Model.Clone();

                notification.Content = "Данные организации полностью удалены!";
                notification.run();
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
                new DirectoryInfo(PathToOrgDirectory).GetFiles()?.ForEach(f => f.Delete());
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

        public Organization model;
        public Organization Model
        {
            get => model;
            set => Set(ref model, value);
        }

        private Organization GetModel() => db.Organizations.FirstOrDefault() ?? new Organization();
        #endregion

        #region Управление файлами лого, печати и подписи
        private const string LOGO_DIRECTORY = "Dental\\Logo";
        private const string STAMP_DIRECTORY = "Dental\\Stamp";
        private const string SIGNATURE_DIRECTORY = "Dental\\Signature";

        private string PathToLogoDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LOGO_DIRECTORY);
        private string PathToStampDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), STAMP_DIRECTORY);
        private string PathToSignatureDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SIGNATURE_DIRECTORY);

        private void ImagesLoading()
        {
            try
            {
                string[] images = new string[] { Model.Logo, Model.Stamp, Model.Signature };
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

      

        private void SaveLogo()
        {
            try
            {
                if (Logo == null) return;
                if (Model?.Logo == ModelBeforeChanges?.Logo) return;
                if (Logo is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.Logo) return;
                }

                if (!string.IsNullOrEmpty(Model.Logo))
                {
                    if (!Directory.Exists(PathToLogoDirectory)) Directory.CreateDirectory(PathToLogoDirectory);                  
                    FileInfo logo = new FileInfo(Model.Logo);
                    if (! logo.Exists) logo.Create();                          
                    logo.CopyTo(Path.Combine(PathToLogoDirectory, logo.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(PathToLogoDirectory, logo.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.Logo = newFile.FullName;

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

        private void SaveStamp()
        {
            try
            {
                if (Stamp == null) return;
                if (Model?.Stamp == ModelBeforeChanges.Stamp) return;
                if (Stamp is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.Stamp) return;
                }

                if (!string.IsNullOrEmpty(Model.Stamp))
                {
                    if (!Directory.Exists(PathToStampDirectory)) Directory.CreateDirectory(PathToStampDirectory);
                    FileInfo stamp = new FileInfo(Model.Stamp);
                    if (!stamp.Exists) stamp.Create();
                    stamp.CopyTo(Path.Combine(PathToStampDirectory, stamp.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(PathToStampDirectory, stamp.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.Stamp = newFile.FullName;

                    // подчищаем директорию. Оставляем только файл, который используется в качестве печати, остальные удаляем.
                    var files = new DirectoryInfo(PathToStampDirectory).GetFiles();
                    foreach (var file in files) if (file.FullName != newFile.FullName) file.Delete();
                    //StampLoading();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void SaveSignature()
        {
            try
            {
                if (Signature == null) return;
                if (Model?.Signature == ModelBeforeChanges.Signature) return;
                if (Signature is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.Signature) return;
                }

                if (!string.IsNullOrEmpty(Model.Signature))
                {
                    if (!Directory.Exists(PathToSignatureDirectory)) Directory.CreateDirectory(PathToSignatureDirectory);
                    FileInfo signature = new FileInfo(Model.Signature);
                    if (!signature.Exists) signature.Create();
                    signature.CopyTo(Path.Combine(PathToSignatureDirectory, signature.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(PathToSignatureDirectory, signature.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.Signature = newFile.FullName;

                    // подчищаем директорию. Оставляем только файл, который используется в качестве печати, остальные удаляем.
                    var files = new DirectoryInfo(PathToSignatureDirectory).GetFiles();
                    foreach (var file in files) if (file.FullName != newFile.FullName) file.Delete();
                    //SignatureLoading();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
       
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
 

                var response = ThemedMessageBox.Show(title: "Внимание", text: msg,
messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

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

        public ImageSource Logo
        {
            get => logo;
            set => Set(ref logo, value);
        }
        private ImageSource logo;

        public ImageSource Stamp
        {
            get => stamp;
            set => Set(ref stamp, value);
        }
        private ImageSource stamp;

        public ImageSource Signature
        {
            get => signature;
            set => Set(ref signature, value);
        }
        private ImageSource signature;
        #endregion

        #region команды, связанных с прикреплением файлов 
        private const string ORG_DIRECTORY = "Dental\\Organization";
        private string PathToOrgDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ORG_DIRECTORY);

        public ICommand DeleteFileCommand { get; }
        public ICommand ExecuteFileCommand { get; }
        public ICommand AttachmentFileCommand { get; }
        public ICommand OpenDirectoryCommand { get; }

        private bool CanDeleteFileCommandExecute(object p) => true;
        private bool CanExecuteFileCommandExecute(object p) => true;
        private bool CanAttachmentFileCommandExecute(object p) => true;
        private bool CanOpenDirectoryCommandExecute(object p) => true;

        private void OnOpenDirectoryCommandExecuted(object p)
        {
            try
            {
                if (Directory.Exists(PathToOrgDirectory)) Process.Start(PathToOrgDirectory);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую файл директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnExecuteFileCommandExecuted(object p)
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

        private void OnAttachmentFileCommandExecuted(object p)
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

                if (!Directory.Exists(PathToOrgDirectory)) Directory.CreateDirectory(PathToOrgDirectory);

                File.Copy(file.FullName, Path.Combine(PathToOrgDirectory, file.Name), true);

                FileInfo newFile = new FileInfo(Path.Combine(PathToOrgDirectory, file.Name));
                newFile.CreationTime = DateTime.Now;

                Files = new DirectoryInfo(PathToOrgDirectory).GetFiles().ToObservableCollection();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteFileCommandExecuted(object p)
        {
            try
            {
                if (p is FileInfo file)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить файл с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    file.Delete();
                    Files = new DirectoryInfo(PathToOrgDirectory).GetFiles().ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ObservableCollection<FileInfo> files;
        public ObservableCollection<FileInfo> Files
        {
            get => files;
            set => Set(ref files, value);
        }
        #endregion
    }
}
