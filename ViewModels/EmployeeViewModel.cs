using System;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using DevExpress.Data.ODataLinq.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using Dental.Services;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.Diagnostics;
using Dental.Models.Base;

namespace Dental.ViewModels
{
    class EmployeeViewModel : ViewModelBase, IImageDeletable
    {
        private ListEmployeesViewModel VmList;

        public EmployeeViewModel(Employee emp, ListEmployeesViewModel vmList)
        {
            db = new ApplicationContext();
            VmList = vmList;
            Model = emp;
            Files = new ObservableCollection<FileInfo>();

            CommunicationList = new List<CommunicationType>() {
                new CommunicationType {  Id=0, Name="Не уведомлять" },
                new CommunicationType {  Id=1, Name="Sms" },
                new CommunicationType {  Id=2, Name="Email" },
                new CommunicationType {  Id=3, Name="Viber" },
            };

            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
            ToggleSwitchedCommand = new LambdaCommand(OnToggleSwitchedCommandExecuted, CanToggleSwitchedCommandExecute);

            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute); 
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);

            #region инициализация команд, связанных с прикреплением файлов сотрудника
            DeleteFileCommand = new LambdaCommand(OnDeleteFileCommandExecuted, CanDeleteFileCommandExecute);
            ExecuteFileCommand = new LambdaCommand(OnExecuteFileCommandExecuted, CanExecuteFileCommandExecute);
            OpenDirectoryCommand = new LambdaCommand(OnOpenDirectoryCommandExecuted, CanOpenDirectoryCommandExecute);
            AttachmentFileCommand = new LambdaCommand(OnAttachmentFileCommandExecuted, CanAttachmentFileCommandExecute);
            #endregion
         
            // подгружаем вспомогательные справочники
            EmployeeGroups = db.EmployeeGroup.ToList();
            Statuses = db.Dictionary.Where(f => f.CategoryId == 6).ToList();
            GenderList = db.Dictionary.Where(f => f.CategoryId == 1).ToList();
            RateType = db.Dictionary.Where(f => f.CategoryId == 7).OrderBy(f => f.Id).ToList();
            Specialities = db.Specialities.ToList();

            RateCheckedStateContent = RateType.Count() > 0 ? RateType[0].Name : "Сдельная оплата";
            RateUncheckedStateContent = RateType.Count() > 0 ? RateType[1].Name : "Фиксированный оклад";

            try
            {
                if (Model.Id == 0)
                {
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
                    EmployeeSpecialities = GetEmployeeSpecialities();
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
                    Title = "Анкета сотрудника (" + Model.Fio + ")";
                     if (Directory.Exists(GetPathToEmpDir()))
                     {
                         Files = new DirectoryInfo(GetPathToEmpDir()).GetFiles().ToObservableCollection();
                     }
                    

                }
                IsNotFixRate = Model?.IsFixRate == 1 ? false : true;
                PhotoLoading();
                
                ModelBeforeChanges = (Employee)Model.Clone();
            }
            catch (Exception e)
            {
                Model = new Employee();
                (new ViewModelLog(e)).run();
            }
        }
        
    #region Блокировка полей
    public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }
        private bool _IsReadOnly;

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

        public bool IsNotFixRate
        {
            get => _IsNotFixRate;
            set => Set(ref _IsNotFixRate, value);
        }
        private bool _IsNotFixRate;

        public ICommand EditableCommand { get; }
        public ICommand ToggleSwitchedCommand { get; }

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

        private bool CanToggleSwitchedCommandExecute(object p) => true;
        private void OnToggleSwitchedCommandExecuted(object p)
        {
            try
            {
                if (Model?.IsFixRate == 1) IsNotFixRate = false;
                else IsNotFixRate = true;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Прикрепление файлов
        #region команды, связанных с прикреплением к карте пациентов файлов 
        private const string EMPLOYEES_DIRECTORY = "Dental\\Employees";
        private string PathToEmployees { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), EMPLOYEES_DIRECTORY);

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
                if (Directory.Exists(GetPathToEmpDir())) Process.Start(GetPathToEmpDir());
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

                string path = GetPathToEmpDir();

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                File.Copy(file.FullName, Path.Combine(path, file.Name), true);

                FileInfo newFile = new FileInfo(Path.Combine(path, file.Name));
                newFile.CreationTime = DateTime.Now;

                var names = new string[] { Model.Fio, "добавлен файл", newFile.Name };
                ActionsLog.RegisterAction(names, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["Employee"]);

                Files = new DirectoryInfo(path).GetFiles().ToObservableCollection();
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
                    var names = new string[] { Model.Fio, "удален файл", file.Name };
                    ActionsLog.RegisterAction(names, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["Employee"]);
                    Files = new DirectoryInfo(GetPathToEmpDir()).GetFiles().ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private string GetPathToEmpDir() => Path.Combine(PathToEmployees, GetGuid());
        private string GetGuid() 
        { 
            if (Model.Guid == null) Model.Guid = KeyGenerator.GetUniqueKey();
            return Model.Guid;
        }

        public ObservableCollection<FileInfo> Files
        {
            get => files;
            set => Set(ref files, value);
        }
        private ObservableCollection<FileInfo> files;
        #endregion
        #endregion

        #region Вспомогательные справочники, заголовок
        public IEnumerable<Dictionary> Statuses { get; set; }
        public IEnumerable<Dictionary> GenderList { get; set; }
        public IEnumerable<EmployeeGroup> EmployeeGroups { get; set; }
        public List<Dictionary> RateType { get; set; }

        public string RateCheckedStateContent { get; set; }
        public string RateUncheckedStateContent { get; set; }

        public string Title { get; set; } = "Анкета сотрудника";

        public List<Speciality> Specialities { get; set; }
        #endregion

        #region Должности сотрудника
        public object EmployeeSpecialities
        {
            get => _EmployeeSpecialities;
            set
            {
                Set(ref _EmployeeSpecialities, value);
            }

        }
        public object _EmployeeSpecialities;

        private List<Speciality> GetEmployeeSpecialities() => Model?.EmployesSpecialities.Where(f => f.EmployeeId == Model?.Id).Select(f => f.Speciality).ToList();
        
        private bool IsEqualEmployeeSpecialities()
        {
            if (EmployeeSpecialities?.ToString()?.Length > 0)
            {
                if (EmployeeSpecialities is List<object> employeeSpecialities)
                {
                    var employeeSpecialitiesFromDb = GetEmployeeSpecialities();

                    if (employeeSpecialities.Count() != employeeSpecialitiesFromDb.Count())
                    {
                        return false;
                    }

                    for (int i = 0; i < employeeSpecialities.Count(); i++)
                    {
                        var es = (Speciality)employeeSpecialities[i];
                        if (!es.Equals(employeeSpecialitiesFromDb[i]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void SaveEmployeeSpecialities()
        {
            try
            {
                if (Model.EmployesSpecialities == null) return;
                if (EmployeeSpecialities is List<object> employeeSpecialities)
                {
                    var col = employeeSpecialities.Cast<Speciality>();
                    foreach (var item in col)
                    {
                        // если эл-т содержится, то пропускаем
                        if (Model.EmployesSpecialities.Select(f => f.Speciality).Contains(item)) continue;

                        //если эл-та нет - то добавляем
                        Model.EmployesSpecialities.Add(new EmployesSpecialities()
                        {
                            Guid = KeyGenerator.GetUniqueKey(),
                            Employee = Model,
                            EmployeeId = Model.Id,
                            EmployeeGuid = Model.Guid,
                            Speciality = item,
                            SpecialityId = item.Id
                        }
                        );
                    }

                    // сравниваем две коллекции, если равно то возврат
                    if (col.Count() == Model.EmployesSpecialities.Count) return;

                    // если не равны, то какие-то элементы убрали из коллекции, находим их и удаляем.
                    var removedSpecialities = Model.EmployesSpecialities.Select(f => f.Speciality).Except(col).ToArray();

                    for (int i = 0; i < removedSpecialities.Count(); i++)
                    {
                        var item = Model.EmployesSpecialities.Where(f => f.Speciality.Id == removedSpecialities[i].Id).FirstOrDefault();
                        if (item == null) continue;
                        db.Entry(item).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Управление моделью
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanSaveCommandExecute(object p) => true;
        private bool CanDeleteCommandExecute(object p) => true;

        public Employee ModelBeforeChanges { get; set; }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = new List<string>();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;

            if (!IsEqualEmployeeSpecialities())
            {
                hasUnsavedChanges = true;
                Model.FieldsChanges.Add("Должности сотрудника");
            }
            return hasUnsavedChanges;
        }

        public Employee _Model;
        public Employee Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }

        private Employee GetModel(int id = 0) => id != 0 ? db.Employes.Where(f => f.Id == id)
            .Include("EmployesSpecialities")
            .FirstOrDefault() : new Employee();

        public bool UserSelectedBtnCancel()
        {
            string warningMessage = "\nПоля: ";
            foreach (var fieldName in Model.FieldsChanges)
            {
                warningMessage += " " + "\"" + fieldName + "\"" + ",";
            }
            warningMessage = warningMessage.Remove(warningMessage.Length - 1);

            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения!" + warningMessage + "\nПродолжить без сохранения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            return response.ToString() == "No";
        }

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) 
                {
                    db.Employes.Add(Model);
                    VmList?.Collection?.Add(Model);
                    ActionsLog.RegisterAction(Model.Fio, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["Employee"]);
                    BtnAfterSaveEnable = true;
                }
                else
                {
                    db.Entry(Model).State = EntityState.Modified;
                    ActionsLog.RegisterAction(Model.Fio, ActionsLog.ActionsRu["edit"], ActionsLog.SectionPage["Employee"]);
                }
                SaveEmployeeSpecialities();
                SavePhoto();

                int cnt = db.SaveChanges();
                notification.Content = "Изменения сохранены в базу данных!";

                if (cnt > 0)
                {
                    Model.UpdateFields();
                    notification.run();
                    ModelBeforeChanges = (Employee)Model.Clone();
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
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить анкету сотрудника из базы данных, без возможности восстановления? Также будут удалены все прикрепленные к анкете сотрудника файлы!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                DeleteEmployeeFiles();

                db.NotificationsLog.Where(f => f.EmployeeId == Model.Id).ToArray()
                    .ForEach(f => db.Entry(f).State = EntityState.Deleted);
                
                db.Entry(Model).State = EntityState.Deleted;
                ActionsLog.RegisterAction(Model.Fio, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["Employee"]);
                int cnt = db.SaveChanges();

                if (cnt > 0) 
                {
                    var notification = new Notification();
                    notification.Content = "Анкета сотрудника полностью удалена из базы данных!";
                    notification.run();
                    if (Application.Current.Resources["Router"] is Navigator nav) nav.LeftMenuClick.Execute("Dental.Views.EmployeeDir.Employes");
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void DeleteEmployeeFiles()
        {
            try
            {
                string path = Path.Combine(PathToEmployees, Model.Guid);
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Неудачная попытка удалить файлы, прикрепленные к анкете сотрудника. Возможно файлы были запущены в другой программе! Попробуйте закрыть запущенные сторонние программы и повторить!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Error);
            }
        }
        #endregion

        #region Управление фото

        public ImageSource Image
        {
            get => image;
            set => Set(ref image, value);
        }
        public ImageSource image;

        private string GetPathToPhoto() => Path.Combine(GetPathToEmpDir(), "Logo");

        private void PhotoLoading()
        {
            try
            {               
                if (!string.IsNullOrEmpty(Model.Photo) && File.Exists(Model.Photo))
                {
                    using (var stream = new FileStream(Model.Photo, FileMode.Open))
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

        private void SavePhoto()
        {
            try
            {
                if (Image == null) return;
                if (Image is BitmapImage img)
                {
                    if (((FileStream)img?.StreamSource)?.Name == Model?.Photo) return;
                }

                if (!string.IsNullOrEmpty(Model.Photo))
                {
                    if (!Directory.Exists(GetPathToPhoto())) Directory.CreateDirectory(GetPathToPhoto());
                    FileInfo logo = new FileInfo(Model.Photo);
                    if (!logo.Exists) logo.Create();
                    logo.CopyTo(Path.Combine(GetPathToPhoto(), logo.Name), true);

                    FileInfo newFile = new FileInfo(Path.Combine(GetPathToPhoto(), logo.Name));
                    newFile.CreationTime = DateTime.Now;
                    Model.Photo = newFile.FullName;

                    // подчищаем директорию. Оставляем только файл, который используется в качестве фото, остальные удаляем.
                    var files = new DirectoryInfo(GetPathToPhoto()).GetFiles();
                    foreach (var file in files) if (file.FullName != newFile.FullName) file.Delete();
                    PhotoLoading();
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
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить файл фото сотрудника?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;
                if (Directory.Exists(GetPathToPhoto()))
                {
                    new DirectoryInfo(GetPathToPhoto()).GetFiles()?.ForEach(f => f.Delete());
                }

                if (p is Infrastructures.Extensions.ImageEditEx ie) ie.Clear();
                PhotoLoading();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

    private readonly ApplicationContext db;
    public List<CommunicationType> CommunicationList { get; }
}
    public class CommunicationType
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
