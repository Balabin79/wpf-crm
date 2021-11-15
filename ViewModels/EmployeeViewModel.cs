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
            

            try
            {
                if (! int.TryParse(employeeId.ToString(), out int id) || id == 0)
                {
                    Model = GetModel(0);
                    Title = "Новый сотрудник";
                    IsReadOnly = false;
                    _BtnIconEditableHide = false;
                    _BtnIconEditableVisible = true;
                }
                else
                {
                    Model = GetModel(id);
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
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
                Title = "Новый сотрудник";
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

        public Employee _Model;
        public Employee Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }

        public string Title { get; set; }

        private Employee GetModel(int id = 0) => id != 0 ? db.Employes.Where(f => f.Id == id).FirstOrDefault() : new Employee();



        /// ///////////////////////////////////////////////////////



        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }
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


                if (Model.Id == 0)
                {
                    //.Add(Model);
                    db.SaveChanges();

                    new Notification().run();
                    return;
                }
                db.Entry(Model).State = EntityState.Modified;
                db.SaveChanges();            
            }
            catch (DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors.First().ValidationErrors.Select(f => f.ErrorMessage);

                if (errors.Count() > 0) { 

                }
            }
            catch (Exception e){}
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


        public IEnumerable<string> Statuses { get; } = new List<string>() { "Работает", "Уволен", "В отпуске", "Другое" };
        public IEnumerable<string> GenderList { get; } = new List<string> { "Мужчина", "Женщина" };
        public IEnumerable<string> RateTypes { get; } = new List<string> { "Фиксированный оклад", "Сдельная оплата" };
    }
}
