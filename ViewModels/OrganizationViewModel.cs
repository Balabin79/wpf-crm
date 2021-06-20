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
using Dental.Infrastructures.Logs;
using Dental.Interfaces.Template;
using Dental.Models;
using Dental.Models.Base;
using Dental.Repositories;
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
            CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
            EditableCommand = new LambdaCommand(OnEditableCommandExecuted, CanEditableCommandExecute);
            IsReadOnly = true;
            try
            {
                db = new ApplicationContext();
                Model = GetModel();
                OldModel = new Organization();
                OldModel.Copy(Model);
                Model.Image = !string.IsNullOrEmpty(Model.Logo) && File.Exists(Model.Logo) ? new BitmapImage(new Uri(Model.Logo)) : null;
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand EditableCommand { get; }
        public ICommand CancelCommand { get; }
       
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanEditableCommandExecute(object p) => true;
        private bool CanCancelCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            try {
                if (Model?.Id > 0) db.Entry(Model).State = EntityState.Modified;
                else db.Organizations.Add(Model);
                db.SaveChanges();
                OldModel.Copy(Model);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelCommandExecuted(object p)
        {
            PropertyInfo[] properties = typeof(Organization).GetProperties();
            bool isModified = false;
            foreach (PropertyInfo property in properties)
            {
                if (!Model[property, OldModel])
                {
                    //if ()
                    isModified = true;
                } 
                                                                    
            } 
            if (isModified == true)
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Есть несохраненные изменения, которые будут утеряны! Продолжить?",
                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (response.ToString() == "No") return;
            }

            Model.Copy(OldModel);
            db.SaveChanges();            
        }  
        
        private void OnEditableCommandExecuted(object p)
        {
            IsReadOnly = !IsReadOnly;
        }


        private Organization _Model;
        public Organization Model { 
            get => _Model; 
            set => Set(ref _Model, value); 
        }

        private bool _IsReadOnly;
        public bool IsReadOnly {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }

        private Organization GetModel() => db.Organizations.FirstOrDefault();
        private Organization OldModel { get; set; }
    }
}
