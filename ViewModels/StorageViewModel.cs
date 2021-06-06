﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;
using Dental.Enums;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.Nomenclatures.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;

namespace Dental.ViewModels
{
    class StorageViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public StorageViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с данным разделом!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<Storage>() { Model });
                else Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetItemChilds().OfType<Storage>().ToObservableCollection());
                db.SaveChanges();
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
                if (SelectedEmployee != null) Model.EmployeeId = ((Employee)SelectedEmployee).Id;

                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Id != f.Id).ToList();

                if (SelectedStorageGroup != null) Model.ParentId = ((Storage)SelectedStorageGroup).Id;

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    new TryingCreatingDuplicate().run(Model.IsDir);
                    return;
                }

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();

                SelectedStorageGroup = null;
                Window.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormCommandExecuted(object p)
        {
            try
            {
                CreateNewWindow();
                if (p == null) return;
                int.TryParse(p.ToString(), out int param);
                if (param == -3) return;

                switch (param)
                {
                    case -1:
                        Model = CreateNewModel();
                        SelectedEmployee = null;
                        Model.IsDir = 0;
                        Title = "Создать склад";
                        StorageGroup = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Title = "Создать группу складов";
                        Model.IsDir = 1;
                        StorageGroup = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        StorageGroup = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<Storage>().ToObservableCollection();

                        SelectedStorageGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();
                       
                        if (Model.EmployeeId != null) SelectedEmployee = db.Employes.Where(f => f.Id == Model.EmployeeId).FirstOrDefault();

                        if (Model.IsDir == 0)
                        {
                            Title = "Редактировать склад";
                            VisibleItemForm();
                        }
                        else
                        {
                            Title = "Редактировать группу складов";
                            VisibleItemGroup();
                        }
                        break;
                }

                Window.DataContext = this;
                Window.ShowDialog();
                SelectedStorageGroup = null;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormCommandExecuted(object p) => Window.Close();

        /************* Специфика этой ViewModel ******************/
        public Employee Employee { get; set; }

        public ICollection<Storage> StorageGroup { get; set; }

        private object _SelectedStorageGroup;
        public object SelectedStorageGroup
        {
            get => _SelectedStorageGroup;
            set => Set(ref _SelectedStorageGroup, value);
        }

        private object _SelectedEmployee;
        public object SelectedEmployee
        {
            get => _SelectedEmployee;
            set => Set(ref _SelectedEmployee, value);
        }

        public ObservableCollection<Employee> Employees { get => db.Employes.OrderBy(d => d.LastName).ToObservableCollection(); }

        /******************************************************/
        public ObservableCollection<Storage> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public Storage Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Height = 330;
        }
        private void VisibleItemGroup()
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Height = 280;
        }

        private ObservableCollection<Storage> _Collection;
        private StorageWindow Window;
        private ObservableCollection<Storage> GetCollection() => db.Storage.OrderBy(d => d.Name).ToObservableCollection();
        private void CreateNewWindow() => Window = new StorageWindow();
        private Storage CreateNewModel() => new Storage();

        private Storage GetModelById(int id)
        {
            return Collection.Where(f => f.Id == id).FirstOrDefault();
        }

        private void Add()
        {
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
            Collection.Add(Model);
        }
        private void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
            var index = Collection.IndexOf(Model);
            if (index != -1) Collection[index] = Model;
        }

        private void Delete(ObservableCollection<Storage> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }
    }
}
