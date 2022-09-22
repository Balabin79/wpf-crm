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
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dental.ViewModels.EmployeeDir
{
    class EmployeeViewModel : DevExpress.Mvvm.ViewModelBase, IImageDeletable
    {
        private readonly ListEmployeesViewModel VmList;
        public readonly ApplicationContext db;

        public delegate void ChangeReadOnly(bool status);
        public event ChangeReadOnly EventChangeReadOnly;

        // bool чтобы знать есть ли изменения в другой вкладке (для показа уведомления)
        public delegate bool SaveCard(Employee employee);
        public event SaveCard EventSaveCard;

        public EmployeeViewModel(Employee emp, ListEmployeesViewModel vmList)
        {
            try
            {
                db = new ApplicationContext();
                VmList = vmList;
                Model = emp != null ? db.Employes.FirstOrDefault(f => f.Id == emp.Id) ?? new Employee() : new Employee();
                EmployeeInfoViewModel = new EmployeeInfoViewModel(Model);
                UserFiles = new UserFilesManagement(Model.Guid);

                Document = new EmployeesDocumentsViewModel();
                IsReadOnly = Model.Id != 0;
                AdditionalFieldsVisible = Visibility.Hidden;
                IsReadOnly = Model.Id != 0;
                PhotoLoading();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с анкетой сотрудника!",
                messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanEditable() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeEditable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeEditable;
        public bool CanDelete() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeDeletable;

        [Command]
        public void Editable()
        {
            IsReadOnly = !IsReadOnly;
            EventChangeReadOnly?.Invoke(IsReadOnly || Model?.Id == 0);
        }

        [Command]
        public void Save()
        {
            try
            {
                bool notificationShowed = false;                                                           
                EmployeeInfoViewModel.Copy(Model);
                SavePhoto();
                if (Model.Id == 0) // новый элемент
                {
                    db.Employes.Add(Model);                
                    // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то тогда добавить
                    if (VmList?.IsArchiveList == Model.IsInArchive) VmList?.Collection?.Add(Model);
                    db.SaveChanges();
                    EventChangeReadOnly?.Invoke(false); // разблокировать команды счетов
                    new Notification() { Content = "Новый сотрудник успешно записан в базу данных!" }.run();
                    notificationShowed = true;
                }
                else
                { // редактирование су-щего эл-та
                    if (db.SaveChanges() > 0)
                    {
                        // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то поменять элемент(отображение изменений), иначе просто добавить
                        if (VmList?.IsArchiveList == Model.IsInArchive)
                        {
                            var item = VmList?.Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item == null) VmList?.Collection?.Add(Model); // добавляем
                            else // меняем
                            {
                                VmList?.Collection?.Remove(item);
                                VmList?.Collection?.Add(Model);
                            }
                        }
                        else // иначе если статусы отличаются (допустим убрали анкету в архив), то только удалить из отображаемого списка
                        {
                            var item = VmList?.Collection.FirstOrDefault(f => f.Id == Model.Id);
                            if (item != null)
                            {
                                VmList?.Collection?.Remove(item);
                            }
                        }
                        new Notification() { Content = "Отредактированные данные сохранены в базу данных!" }.run();
                        notificationShowed = true;
                    }
                }
                if (Model != null)
                {
                    if (EventSaveCard?.Invoke(Model) == true && !notificationShowed) new Notification() { Content = "Отредактированные данные сохранены в базу данных!" }.run();
                }
                SetTabIndividualPriceVisibility();
            }
            catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с анкетой сотрудника!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить анкету сотрудника из базы данных, без возможности восстановления? Также будут удалены записи в расписании и все файлы прикрепленные к анкете сотрудника!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Model.Guid).DeleteDirectory();
                var id = Model?.Id;
                //удалить также в расписании и в счетах
                db.Appointments.Where(f => f.EmployeeId == Model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.AdditionalEmployeeValue.Where(f => f.EmployeeId == Model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Анкета сотрудника полностью удалена из базы данных!" }.run();

                // может не оказаться этого эл-та в списке, например, он в статусе "В архиве"
                var item = VmList?.Collection.FirstOrDefault(f => f.Id == Model.Id);
                if (item != null) VmList?.Collection.Remove(item);

                db.SaveChanges();
                VmList?.EmployeeWin.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении анкеты сотрудника произошла ошибка, перейдите в раздел \"Сотрудники\"!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (EmployeeInfoViewModel.FieldsChanges != null) EmployeeInfoViewModel.FieldsChanges = EmployeeInfoViewModel.CreateFieldsChanges();
            if (!EmployeeInfoViewModel.Equals(Model)) hasUnsavedChanges = true;
            return hasUnsavedChanges;
        }

        public bool UserSelectedBtnCancel()
        {
            string fieldNames = "";
            var warningMessage = "\n";
            foreach (var prop in EmployeeInfoViewModel.FieldsChanges)
            {
                fieldNames += " \"" + prop + "\", ";
            }
            if (fieldNames.Length > 3) warningMessage += "Поля:" + fieldNames.Remove(fieldNames.Length - 2) + "\n";
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения!" + warningMessage + "\nПродолжить без сохранения?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            return response.ToString() == "No";
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Employee Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public EmployeeInfoViewModel EmployeeInfoViewModel
        {
            get { return GetProperty(() => EmployeeInfoViewModel); }
            set { SetProperty(() => EmployeeInfoViewModel, value); }

        }

        public UserFilesManagement UserFiles
        {
            get { return GetProperty(() => UserFiles); }
            set { SetProperty(() => UserFiles, value); }
        }

        public EmployeesDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
        }

        public ICollection<string> GenderList { get => _GenderList; }
        private readonly ICollection<string> _GenderList = new List<string> { "Мужчина", "Женщина" };

        public Visibility AdditionalFieldsVisible
        {
            get { return GetProperty(() => AdditionalFieldsVisible); }
            set { SetProperty(() => AdditionalFieldsVisible, value); }
        }

        public Visibility IndividualPriceVisible
        {
            get { return GetProperty(() => IndividualPriceVisible); }
            set { SetProperty(() => IndividualPriceVisible, value); }
        }

        public void SetTabVisibility(Visibility visibility) => AdditionalFieldsVisible = visibility;

        public void SetTabIndividualPriceVisibility() => IndividualPriceVisible = Model?.UseIndividualPrice == 1 ? Visibility.Visible : Visibility.Hidden;

        private bool ContactNeedUpdate()
        {
            try
            {
                if (Model.Id == 0) return false;
                return EmployeeInfoViewModel.FirstName != Model.FirstName || EmployeeInfoViewModel.LastName != Model.LastName || EmployeeInfoViewModel.MiddleName != Model.MiddleName || EmployeeInfoViewModel.BirthDate != Model.BirthDate || EmployeeInfoViewModel.Phone != Model.Phone || EmployeeInfoViewModel.Sex != Model.Sex || EmployeeInfoViewModel.Email != Model.Email;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #region Управление фото

        private string GetPathToPhoto() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6\\Files", Model?.Guid, "Photo");
        
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
                        EmployeeInfoViewModel.Image = img;                       
                    }
                }
                else EmployeeInfoViewModel.Image = null;
                EmployeeInfoViewModel.Photo = Model.Photo;

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
                if (EmployeeInfoViewModel?.Image == null) return;
                if (EmployeeInfoViewModel?.Image is BitmapImage img)
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
                var model = VmList?.Collection?.FirstOrDefault(f => f.Id == Model.Id);
                if (model != null) 
                { 
                    model.Image = null;
                    model.Photo = null;
                    if (db.SaveChanges() > 0) new Notification() { Content = "Фото сотрудника удалено!" }.run();
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
