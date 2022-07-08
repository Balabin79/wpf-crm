using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.AdditionalFields;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels.AdditionalFields
{
    public class AdditionalEmployeeFieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public AdditionalEmployeeFieldsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
                Templates = db.TemplateType.ToArray();
            }
            catch
            {

            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is AdditionalEmployeeField model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0)
                    {
                        db.AdditionalEmployeeFields.Remove(model);
                        if (db.SaveChanges() > 0) new Notification() { Content = "Успешно удалено из базы данных!" }.run();
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Detached;
                    }
                    Collection?.Remove(model);
                }
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке удаления произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Save()
        {
            try
            {
                foreach (var item in Collection)
                {
                    if (string.IsNullOrEmpty(item.Label) || string.IsNullOrEmpty(item.SysName)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                if (db.SaveChanges() > 0)
                {
                    SetCollection();
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранения изменений в базу данных, произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Add() => Collection.Add(new AdditionalEmployeeField());

        private void SetCollection() => Collection = db.AdditionalEmployeeFields.OrderBy(f => f.Label).Include(f => f.TypeValue).ToObservableCollection();

        public bool HasUnsavedChanges()
        {
      
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Label) || string.IsNullOrEmpty(item.SysName)) continue;
                if (item.Id == 0) return true;

            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            return response.ToString() == "No";
        }



        public AdditionalClientField Model { get; private set; }
        public ICollection<TemplateType> Templates { get; private set; }

        public ObservableCollection<AdditionalEmployeeField> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

    }
}
