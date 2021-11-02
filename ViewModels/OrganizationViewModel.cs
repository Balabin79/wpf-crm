using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class OrganizationViewModel: ViewModelBase
    {
        private readonly ApplicationContext db;
       
        public OrganizationViewModel()
        {
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            IsReadOnly = true;
            try
            {
                db = new ApplicationContext();
                Model = GetModel();
                Model.Image = !string.IsNullOrEmpty(Model.Logo) && File.Exists(Model.Logo) ? new BitmapImage(new Uri(Model.Logo)) : null;
                if (Model != null)
                {
                    IsReadOnly = true;
                    _BtnIconEditableHide = true;
                    _BtnIconEditableVisible = false;
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
                else db.Entry(Model).State = EntityState.Modified;
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
                ModelBeforeChanges.Copy(Model);

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
        { 
            db.Entry(Model).State = EntityState.Deleted;
            db.SaveChanges();
        }
    }
}
