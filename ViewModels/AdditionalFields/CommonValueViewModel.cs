using System;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using System.Collections.Generic;
using Dental.ViewModels.Materials;
using Dental.Views.AdditionalFields;

namespace Dental.ViewModels.AdditionalFields
{
    public class CommonValueViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public CommonValueViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCommonValues();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Общие значения\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanAdd() => true;
        public bool CanDelete(object p) => true;
        public bool CanSave() => true;

        [Command]
        public void Add() => CommonValues?.Add(new CommonValue());

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is CommonValue model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0)
                    {
                        db.CommonValues.Remove(model);
                        if (db.SaveChanges() > 0) new Notification() { Content = "Успешно удалено из базы данных!" }.run();
                    }
                    else 
                    { 
                        db.Entry(model).State = EntityState.Detached;
                    }
                    CommonValues?.Remove(model);
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void Save()
        {
            try
            {
                foreach (var item in CommonValues)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;                    
                }
                if (db.SaveChanges() > 0) 
                {
                    SetCommonValues();
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run(); 
                }            
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public bool HasUnsavedChanges()
        {
          //  if (CollectionBeforeChanges?.Count != MaterialViewModel?.Measures?.Count) return true;
            foreach (var item in CommonValues)
            {
                if (string.IsNullOrEmpty(item.Name)) continue;
                if (item.Id == 0) return true;
                //if (!item.Equals(CollectionBeforeChanges.Where(f => f.Guid == item.Guid).FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning) ;
            return response.ToString() == "No";
        }

        public ICollection<CommonValue> CommonValues
        {
            get { return GetProperty(() => CommonValues); }
            set { SetProperty(() => CommonValues, value); }
        }

        public void SetCommonValues() => CommonValues = db.CommonValues.ToObservableCollection() ?? new ObservableCollection<CommonValue>();

    }
}
