using System;
using System.Linq;
using System.Windows.Input;
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
using DevExpress.Mvvm.DataAnnotations;
using Dental.Services.Files;

namespace Dental.ViewModels
{
    class EmployeeViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable
    {
        private readonly ListEmployeesViewModel VmList;

        public EmployeeViewModel(Employee emp, ListEmployeesViewModel vmList)
        {
            db = new ApplicationContext();
            VmList = vmList;
            Model = emp;
            Files = new ObservableCollection<FileInfo>();
            Document = new EmployeesDocumentsViewModel();
              
         
            // подгружаем вспомогательные справочники
            Statuses = db.Dictionary.Where(f => f.CategoryId == 6).ToList();
            GenderList = db.Dictionary.Where(f => f.CategoryId == 1).ToList();


            try
            {
                IsReadOnly = Model.Id != 0;
                if (Directory.Exists(GetPathToEmpDir())) Files = new DirectoryInfo(GetPathToEmpDir()).GetFiles().ToObservableCollection();  
                PhotoLoading();               
                ModelBeforeChanges = (Employee)Model.Clone();
            }
            catch (Exception e)
            {
                Model = new Employee();
                (new ViewModelLog(e)).run();
            }
        }

        public EmployeesDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
        }

        #region Блокировка полей
        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        [Command]
        public void Editable() => IsReadOnly = !IsReadOnly;

        #endregion

        #region команды, связанных с прикреплением к карте пациентов файлов 
        private const string EMPLOYEES_DIRECTORY = "Dental\\Employees";
        private string PathToEmployees { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), EMPLOYEES_DIRECTORY);

        [Command]
        public void OpenDirectory()
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
        public void AttachmentFile()
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

                FileInfo newFile = new FileInfo(Path.Combine(path, file.Name)) { CreationTime = DateTime.Now };

                var names = new string[] { Model.Fio, "добавлен файл", newFile.Name };

                Files = new DirectoryInfo(path).GetFiles().ToObservableCollection();
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
                    var names = new string[] { Model.Fio, "удален файл", file.Name };
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
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }
        #endregion

        #region Вспомогательные справочники, заголовок
        public IEnumerable<Dictionary> Statuses { get; set; }
        public IEnumerable<Dictionary> GenderList { get; set; }
        public List<Dictionary> RateType { get; set; }

        public string RateCheckedStateContent { get; set; }
        public string RateUncheckedStateContent { get; set; }

        public string Title { get; set; } = "Анкета сотрудника";
        #endregion

        #region Управление моделью

        public Employee ModelBeforeChanges { get; set; }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = new List<string>();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;

            return hasUnsavedChanges;
        }

        public Employee Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
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

        [Command]
        public void Save()
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0)
                {
                    db.Employes.Add(Model);
                    VmList?.Collection?.Add(Model);
                }
                else
                {
                    db.Entry(Model).State = EntityState.Modified;
                }
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

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить анкету сотрудника из базы данных, без возможности восстановления? Также будут удалены все прикрепленные к анкете сотрудника файлы!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                DeleteEmployeeFiles();
                
                db.Entry(Model).State = EntityState.Deleted;

                if (db.SaveChanges() > 0) new Notification() { Content = "Анкета сотрудника полностью удалена из базы данных!" }.run();
                               
                if (Application.Current.Resources["Router"] is Navigator nav) nav.LeftMenuClick("Dental.Views.EmployeeDir.Employes");
                
                VmList.EmployeeWin.Close();                                       
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты сотрудника произошла ошибка, перейдите в раздел \"Сотрудники\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
            get { return GetProperty(() => Image); }
            set { SetProperty(() => Image, value); }
        }

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

                    FileInfo newFile = new FileInfo(Path.Combine(GetPathToPhoto(), logo.Name)) { CreationTime = DateTime.Now };
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
}

}
