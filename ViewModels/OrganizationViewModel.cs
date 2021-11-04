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
using Dental.Services;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class OrganizationViewModel : ViewModelBase
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
                Files = new ObservableCollection<ClientFiles>();
                Model = GetModel();
                LogoLoading();
                if (Model != null)
                {
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
                    if (ProgramDirectory.HasOrgDirectoty())
                    {
                        Files = ProgramDirectory.GetFilesFromOrgDirectory().ToObservableCollection<ClientFiles>();
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

        public ICommand SaveCommand { get; }
       
        private bool CanSaveCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            try { 
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) db.Organizations.Add(Model);
                else SaveFiles();
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

        public ICommand DeleteCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;

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

        public Organization ModelBeforeChanges { get; set; }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = new List<string>();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            if (HasUnsavedFiles()) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

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

        public Organization _Model;
        public Organization Model 
        {
            get => _Model;
            set => Set(ref _Model, value);
        }


        private Organization GetModel() => db.Organizations.FirstOrDefault() ?? new Organization();
        private void Delete()
        { try
            {
                db.Entry(Model).State = EntityState.Deleted;
                db.SaveChanges();
                ProgramDirectory.RemoveAllOrgFiles();
                Files = new ObservableCollection<ClientFiles>();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }

        }

        /////////////////////////////

        #region команды, связанных с прикреплением к карте пациентов файлов     
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
                var file = p as ClientFiles;
                var dir = Path.GetDirectoryName(file?.Path);
                Process.Start(dir);
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
                var file = p as ClientFiles;
                Process.Start(file?.Path);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно запустить файл!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAttachmentFileCommandExecuted(object p)
        {
            try
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
            }
        }

        private void OnDeleteFileCommandExecuted(object p)
        {
            try
            {
                var file = p as ClientFiles;
                var item = Files.Where<ClientFiles>(f => string.Compare(f.FullName, file.FullName, StringComparison.CurrentCulture) == 0).FirstOrDefault();
                if (item == null) return;
                if (string.Compare(file.Status, ClientFiles.STATUS_SAVE_RUS, StringComparison.CurrentCulture) == 0)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Вы собираетесь физически удалить файл с компьютера! Вы уверены в своих действиях?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    ProgramDirectory.RemoveFileFromOrgDirectory(file);
                }
                Files.Remove(file);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool FindDoubleFile(string fileName)
        {
            foreach (var file in Files)
            {
                if (string.Compare(file.FullName, fileName, StringComparison.CurrentCulture) == 0) return true;
            }
            return false;
        }

        private bool SaveFiles()
        {
            try
            {
                //если нет директории программы, то создаем ее
                if (!ProgramDirectory.HasMainProgrammDirectory())
                {
                    var _ = ProgramDirectory.CreateMainProgrammDirectoryForPatientCards();
                }
                if (!ProgramDirectory.HasOrgDirectoty())
                {
                    var _ = ProgramDirectory.CreateOrgDirectory();
                }

                ProgramDirectory.Errors.Clear();
                MoveFilesToOrgDirectory();
                return true;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
                return false;
            }
        }

        private void SaveLogo()
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.Logo))
                {
                    if (!ProgramDirectory.HasLogoDirectoty()) ProgramDirectory.CreateLogoDirectory();
                    if (Path.GetDirectoryName(Model.Logo) != ProgramDirectory.GetPathLogoDirectoty())
                    {
                        ProgramDirectory.SaveFileInLogoDirectory(new ClientFiles() { Name = Path.GetFileName(Model.Logo), Path = Model.Logo });
                        Model.Logo = Path.Combine(ProgramDirectory.GetPathLogoDirectoty(), (Path.GetFileName(Model.Logo)));
                    }
                }                
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void MoveFilesToOrgDirectory()
        {
            foreach (ClientFiles file in Files)
            {
                if (string.Compare(file.Status, ClientFiles.STATUS_SAVE_RUS, StringComparison.CurrentCulture) == 0) continue;
                ProgramDirectory.SaveInOrgDirectory(file);
                file.Status = ClientFiles.STATUS_SAVE_RUS;
            }
        }
        
        public bool HasUnsavedFiles()
        {
            if (Files.Count > 0)
            {
                foreach (var i in Files)
                {
                    if (i.Status == ClientFiles.STATUS_NEW_RUS)
                    {
                        Model.FieldsChanges.Add("Прикрепляемые файлы");
                        return true;
                    }
                }
            }
            return false;
        }

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
        
        public ObservableCollection<ClientFiles> _Files;
        public ObservableCollection<ClientFiles> Files
        {
            get => _Files;
            set => Set(ref _Files, value);
        }
        #endregion

        public ImageSource Image 
        {
            get => _Image;
            set => Set(ref _Image, value); 
        }
        public ImageSource _Image;

    }
}
