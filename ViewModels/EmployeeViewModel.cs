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

namespace Dental.ViewModels
{
    class EmployeeViewModel : ViewModelBase
    {
        public EmployeeViewModel() : this(0) { }

        public EmployeeViewModel(int? employeeId = 0)
        {

            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
            ToggleSwitchedCommand = new LambdaCommand(OnToggleSwitchedCommandExecuted, CanToggleSwitchedCommandExecute);

            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);

            db = new ApplicationContext();
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
                if (!int.TryParse(employeeId.ToString(), out int id) || id == 0)
                {
                    Model = GetModel(0);
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
                    Model = GetModel(id);
                    EmployeeSpecialities = GetEmployeeSpecialities();
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
                    Title = "Анкета сотрудника (" + Model.Fio + ")";
                    /* if (ProgramDirectory.HasOrgDirectoty())
                     {
                         Files = ProgramDirectory.GetFilesFromOrgDirectory().ToObservableCollection<ClientFiles>();
                     }
                    */

                }
                IsNotFixRate = Model?.IsFixRate == 1 ? false : true;
                //PhotoLoading();
                Image = !string.IsNullOrEmpty(Model.Photo) && File.Exists(Model.Photo) ? new BitmapImage(new Uri(Model.Photo)) : null;

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

        private List<Speciality> GetEmployeeSpecialities()
        {
            return Model?.EmployesSpecialities.Where(f => f.EmployeeId == Model?.Id).Select(f => f.Speciality).ToList();
        }

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

            if (HasUnsavedFiles()) hasUnsavedChanges = true;
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

            var response = ThemedMessageBox.Show(title: "Внимание", text: "В форме организации имеются несохраненные изменения! Если вы не хотите их потерять, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой).\nИзменения:" + warningMessage,
               messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

            return response.ToString() == "Cancel";
        }

        public ICommand SaveCommand { get; }
        private bool CanSaveCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) db.Employes.Add(Model);
                SaveEmployeeSpecialities();
                //else SaveFiles();
                //SavePhoto();

                db.SaveChanges();

                notification.Content = "Изменения сохранены в базу данных!";

                if (HasUnsavedChanges())
                {
                    ////////////////
                    ///

                    notification.run();
                    ModelBeforeChanges = (Employee)Model.Clone();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Управление фото
        public ImageSource Image
        {
            get => _Image;
            set => Set(ref _Image, value);
        }
        public ImageSource _Image;

        #region Управление фото
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
        /*
        private void SavePhoto()
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.Photo))
                {
                    if (!ProgramDirectory.HasLogoDirectoty()) ProgramDirectory.CreateLogoDirectory();
                    if (Path.GetDirectoryName(Model.Photo) != ProgramDirectory.GetPathLogoDirectoty())
                    {
                        ProgramDirectory.SaveFileInLogoDirectory(new ClientFiles() { Name = Path.GetFileName(Model.Photo), Path = Model.Photo });
                        Model.Photo = Path.Combine(ProgramDirectory.GetPathLogoDirectoty(), (Path.GetFileName(Model.Photo)));
                    }
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }*/
        #endregion
        #endregion

        #region управление фото и файлами сотрудника
        public ObservableCollection<ClientFiles> _Files;
        public ObservableCollection<ClientFiles> Files
        {
            get => _Files;
            set => Set(ref _Files, value);
        }

        public bool HasUnsavedFiles()
        {
            if (Files?.Count > 0)
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
        #endregion

        private readonly ApplicationContext db;

    }
}
