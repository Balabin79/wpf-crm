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
                LogoLoading();
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
                Image = null;
                Files.Clear();
                new DirectoryInfo(PathToOrgDirectory).GetFiles()?.ForEach(f => f.Delete());
                new DirectoryInfo(PathToLogoDirectory).GetFiles()?.ForEach(f => f.Delete());
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
            if (Model.FieldsChanges != null) Model.FieldsChanges = new List<string>();
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

        #region Управление лого
        private const string LOGO_DIRECTORY = "Dental\\Logo";
        private string PathToLogoDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LOGO_DIRECTORY);

        private void LogoLoading()
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.Logo) && File.Exists(Model.Logo))
                {
                    using (var stream = new FileStream(Model.Logo, FileMode.Open))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = stream;
                        img.EndInit();
                        img.Freeze();
                        Image = img;
                    }
                }
                else Image = null;
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
                if (Image == null) return;
                if (Image is BitmapImage img)
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
                    LogoLoading();
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
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить файл логотипа?",
messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;
                if (Directory.Exists(PathToLogoDirectory))
                {
                    new DirectoryInfo(PathToLogoDirectory).GetFiles()?.ForEach(f => f.Delete());
                }
                
                if (p is Infrastructures.Extensions.ImageEditEx ie) ie.Clear();
                LogoLoading();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public ImageSource Image
        {
            get => image;
            set => Set(ref image, value);
        }
        public ImageSource image;
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

            /*try
            {
                var fileContent = string.Empty;
                var filePath = string.Empty;
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                        ClientFiles file = new ClientFiles();
                        file.Path = filePath;
                        file.DateCreated = DateTime.Today.ToShortDateString();
                        file.Name = Path.GetFileNameWithoutExtension(filePath);
                        file.FullName = Path.GetFileName(filePath);
                        if (Path.HasExtension(filePath))
                        {
                            file.Extension = Path.GetExtension(filePath);
                        }
                        file.Size = new FileInfo(filePath).Length.ToString();

                        if (FindDoubleFile(file.FullName))
                        {
                            var response = ThemedMessageBox.Show(title: "Внимание!", text: "Файл с таким именем уже есть в списке прикрепленных файлов. Вы хотите его заменить?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                            if (response.ToString() == "No") return;

                            var idx = Files.IndexOf(f => (string.Compare(f.FullName, file.FullName, StringComparison.CurrentCulture) == 0));
                            if (idx == -1) return;
                            file.Status = ClientFiles.STATUS_NEW_RUS;
                            Files[idx] = file;                            
                            return;
                        }
                        Files.Insert(0, file);
                    }
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }*/
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
