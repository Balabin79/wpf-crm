using System;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
        public EmployeeViewModel() : this(0){}

        public EmployeeViewModel(int? employeeId = 0)
        {

            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
            ///

            CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenCommand = new LambdaCommand(OnOpenCommandExecuted, CanOpenCommandExecute);

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
                if (! int.TryParse(employeeId.ToString(), out int id) || id == 0)
                {
                    Model = GetModel(0);
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
                    Model = GetModel(id);
                    //EmployeeSpecialities = db.EmployesSpecialities.Where(f => f.Guid == Model.Guid).ToList();
                    //IsPieceWork = string.IsNullOrEmpty(Model.RateType) || Model.RateType == "Сдельная оплата";
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
                    Title = "Анкета сотрудника (" + Model.Fio + ")";
                   /* if (ProgramDirectory.HasOrgDirectoty())
                    {
                        Files = ProgramDirectory.GetFilesFromOrgDirectory().ToObservableCollection<ClientFiles>();
                    }
                   */

                    Image = !string.IsNullOrEmpty(Model.Photo) && File.Exists(Model.Photo) ? new BitmapImage(new Uri(Model.Photo)) : null;                
                    }
                ModelBeforeChanges = (Employee)Model.Clone();
            } 
            catch (Exception e)
            {
                Model = new Employee();
                (new ViewModelLog(e)).run();
            }           
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

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
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
        public ImageSource Image
        {
            get => _Image;
            set => Set(ref _Image, value);
        }
        public ImageSource _Image;

        public ObservableCollection<ClientFiles> _Files;
        public ObservableCollection<ClientFiles> Files
        {
            get => _Files;
            set => Set(ref _Files, value);
        }

        public Employee ModelBeforeChanges { get; set; }

        public bool HasUnsavedChanges()
        {
            bool hasUnsavedChanges = false;
            if (Model.FieldsChanges != null) Model.FieldsChanges = new List<string>();
            if (!Model.Equals(ModelBeforeChanges)) hasUnsavedChanges = true;
            if (HasUnsavedFiles()) hasUnsavedChanges = true;
            return hasUnsavedChanges;
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

        public Employee _Model;
        public Employee Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }


        private Employee GetModel(int id = 0) => id != 0 ? db.Employes.Where(f => f.Id == id).FirstOrDefault() : new Employee();


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
        /// ///////////////////////////////////////////////////////



        public ICommand CancelCommand { get; }
        
        public ICommand OpenCommand { get; }


        private void OnCancelCommandExecuted(object p)
        {
            try
            {

               // Repository.Delete(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                if (Model == null) return;
                var notification = new Notification();
                if (Model.Id == 0) db.Employes.Add(Model);
                //else SaveFiles();
                //SaveLogo();
                db.SaveChanges();
                notification.Content = "Изменения сохранены в базу данных!";

                if (HasUnsavedChanges())
                {
                    notification.run();
                    ModelBeforeChanges = (Employee)Model.Clone();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenCommandExecuted(object p)
        {
            int x = 0;
            try
            {

                //Repository.Update(table);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
 
        private readonly ApplicationContext db;


        private bool CanCancelCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenCommandExecute(object p) => true;


        public IEnumerable<Dictionary> Statuses { get; set; } 
        public IEnumerable<Dictionary> GenderList { get; set; } 
        public IEnumerable<EmployeeGroup> EmployeeGroups { get; set; }
        public List<Dictionary> RateType { get; set; }

        public List<Speciality> Specialities { get; set; }
        public object EmployeeSpecialities { get; set; }

        public string RateCheckedStateContent { get; set; }
        public string RateUncheckedStateContent { get; set; }

        public bool IsPieceWork { get; set; } = true;
        public string Title { get; set; } = "Анкета сотрудника";
    }
}
