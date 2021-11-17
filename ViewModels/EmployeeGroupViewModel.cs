﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Grid;
using Dental.Services;
using Dental.Infrastructures.Collection;
using System.Collections.Generic;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class EmployeeGroupViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public EmployeeGroupViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
                Collection.ForEach(f => CollectionBeforeChanges.Add((EmployeeGroup)f.Clone()));
                PercentOrCost = db.Dictionary.Where(f => f.CategoryId == 2).ToList();
                MoreOrLess = db.Dictionary.Where(f => f.CategoryId == 3).ToList();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Группы сотрудников\"!",
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
                if (p is EmployeeGroup model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;

                    Collection.Remove(model);
                    if (model.Id != 0) db.Entry(model).State = EntityState.Deleted;
                    else db.Entry(model).State = EntityState.Detached;
                    db.SaveChanges();
                    CollectionBeforeChanges = new ObservableCollection<EmployeeGroup>();
                    Collection.ForEach(f => CollectionBeforeChanges.Add((EmployeeGroup)f.Clone()));
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
                        item.Guid = KeyGenerator.GetUniqueKey();
                        db.Entry(item).State = EntityState.Added;
                        continue;
                    }
                    else if (string.IsNullOrEmpty(item.Guid)) item.Guid = KeyGenerator.GetUniqueKey();
                }
                int rows = db.SaveChanges();
                Collection.Where(f => f.Id == 0).ToArray().ForEach(f => Collection.Remove(f));

                CollectionBeforeChanges = new ObservableCollection<EmployeeGroup>();
                Collection.ForEach(f => CollectionBeforeChanges.Add((EmployeeGroup)f.Clone()));
                if (rows != 0)
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

        private void OnAddCommandExecuted(object p) => Collection.Add(new EmployeeGroup());

        public ObservableCollection<EmployeeGroup> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<EmployeeGroup> _Collection;
        private ObservableCollection<EmployeeGroup> GetCollection() => db.EmployeeGroup.OrderBy(d => d.Name).ToObservableCollection();
        public ObservableCollection<EmployeeGroup> CollectionBeforeChanges { get; set; } = new ObservableCollection<EmployeeGroup>();

        public bool HasUnsavedChanges()
        {
            if (CollectionBeforeChanges?.Count != Collection.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Name)) continue;
                if (item.Id == 0) return true;
                if (!item.Equals(
                    CollectionBeforeChanges.
                    Where(
                        f => f.Guid == item.Guid)
                    .FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {

            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Если хотите сохранить эти данные, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой). Для продолжения без сохранения, нажмите \"Ок\"",
               messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

            return response.ToString() == "Cancel";
        }

        public List<Dictionary> MoreOrLess { get; set; }
        public List<Dictionary> PercentOrCost { get; set; }
    }
}