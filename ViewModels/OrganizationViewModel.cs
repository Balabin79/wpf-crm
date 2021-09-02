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
            IsReadOnly = true;
            try
            {
                db = new ApplicationContext();
                Model = GetModel();
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
       
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanEditableCommandExecute(object p) => true;

        private void OnSaveCommandExecuted(object p)
        {
            try {
                if (Model?.Id > 0) db.Entry(Model).State = EntityState.Modified;
                else db.Organizations.Add(Model);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        
        private void OnEditableCommandExecuted(object p)
        {
            IsReadOnly = !IsReadOnly;
        }

        public Organization Model { get; set; }

        private bool _IsReadOnly;
        public bool IsReadOnly {
            get => _IsReadOnly;
            set => Set(ref _IsReadOnly, value);
        }

        private Organization GetModel() => db.Organizations.FirstOrDefault();
    }
}
