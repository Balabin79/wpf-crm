using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using System.Collections.Generic;

namespace Dental.ViewModels
{
    class WarehouseViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public WarehouseViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();                
                Collection.ForEach(f => CollectionBeforeChanges.Add((Warehouse)f.Clone()));
                Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Склады\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p is Warehouse model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0) 
                    {
                        db.Entry(model).State = EntityState.Deleted;
                        ActionsLog.RegisterAction(model.Name, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["Warehouse"]);
                        int cnt = db.SaveChanges();
                        if (cnt > 0)
                        {
                            var notification = new Notification();
                            notification.Content = "Успешно удалено из базы данных!";
                            notification.run();
                        }
                    } 
                    else db.Entry(model).State = EntityState.Detached;
                    Collection.Remove(model);
                    CollectionBeforeChanges.Clear();
                    Collection.ForEach(f => CollectionBeforeChanges.Add((Warehouse)f.Clone()));
                }
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
                foreach (var item in Collection)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Id == 0) 
                    {
                        db.Entry(item).State = EntityState.Added;
                        ActionsLog.RegisterAction(item.Name, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["Warehouse"]);
                    }
                    if (db.Entry(item).State == EntityState.Modified)
                    {
                        ActionsLog.RegisterAction(item.Name, ActionsLog.ActionsRu["edit"], ActionsLog.SectionPage["Warehouse"]);
                    }                       
                }
                int cnt = db.SaveChanges();
                Collection = GetCollection();
                CollectionBeforeChanges.Clear();
                Collection.ForEach(f => CollectionBeforeChanges.Add((Warehouse)f.Clone()));
                if (cnt > 0)
                {
                    var notification = new Notification();
                    notification.Content = "Изменения сохранены в базу данных!";
                    notification.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddCommandExecuted(object p) => Collection.Add(new Warehouse());

        public ObservableCollection<Warehouse> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<Warehouse> _Collection;
        private ObservableCollection<Warehouse> GetCollection() => db.Warehouse?.Include(f => f.Employee).OrderBy(d => d.Name).ToObservableCollection();
        public ObservableCollection<Warehouse> CollectionBeforeChanges { get; set; } = new ObservableCollection<Warehouse>();

        public bool HasUnsavedChanges()
        {
            if (CollectionBeforeChanges?.Count != Collection.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Name)) continue;
                if (item.Id == 0) return true;
                if (!item.Equals(CollectionBeforeChanges.Where(f => f.Guid == item.Guid).FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning) ;

            return response.ToString() == "No";
        }

        public ICollection<Employee> Employees { get; }
    }
}
