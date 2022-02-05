﻿using System;
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


namespace Dental.ViewModels
{
    class AdvertisingViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public AdvertisingViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
                
                Collection.ForEach(f => CollectionBeforeChanges.Add((Advertising)f.Clone()));
                Navigator.HasUnsavedChanges = HasUnsavedChanges;
                Navigator.UserSelectedBtnCancel = UserSelectedBtnCancel;
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Рекламные источники\"!",
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
                if (p is Advertising model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0) 
                    {
                        db.Entry(model).State = EntityState.Deleted;
                        ActionsLog.RegisterAction(model.Name, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["Advertising"]);
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
                    Collection.ForEach(f => CollectionBeforeChanges.Add((Advertising)f.Clone()));
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
                        ActionsLog.RegisterAction(item.Name, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["Advertising"]);
                    }
                    if (db.Entry(item).State == EntityState.Modified)
                    {
                        ActionsLog.RegisterAction(item.Name, ActionsLog.ActionsRu["edit"], ActionsLog.SectionPage["Advertising"]);
                    }                       
                }
                int cnt = db.SaveChanges();
                Collection = GetCollection();
                CollectionBeforeChanges.Clear();
                Collection.ForEach(f => CollectionBeforeChanges.Add((Advertising)f.Clone()));
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

        private void OnAddCommandExecuted(object p) => Collection.Add(new Advertising());

        public ObservableCollection<Advertising> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<Advertising> _Collection;
        private ObservableCollection<Advertising> GetCollection() => db.Advertising?.OrderBy(d => d.Name).ToObservableCollection();
        public ObservableCollection<Advertising> CollectionBeforeChanges { get; set; } = new ObservableCollection<Advertising>();

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
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Продолжить без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning) ;

            return response.ToString() == "No";
        }
    }
}
