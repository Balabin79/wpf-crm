using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.ViewModels
{
    class DiscountGroupsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public DiscountGroupsViewModel()
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
                if (Model == null || !new ConfirDeleteInCollection().run(null)) return;
                Delete(new ObservableCollection<DiscountGroups>() { Model });             
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
                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.Name == Model.Name && Model.Id != f.Id).ToList();
                if (SelectedType != null) Model.DiscountGroupType = SelectedType.ToString();

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();
                SelectedType = null;
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
                        Title = "Новая скидка";
                        Group = Collection.Where(f => f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        break;
                    default:
                        Model = GetModelById(param);

                        SelectedType = Model.DiscountGroupType;
                        Title = "Редактировать скидку";
                        break;
                }

                Window.DataContext = this;
                Window.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormCommandExecuted(object p) => Window.Close();

        /************* Специфика этой ViewModel ******************/
        public ICollection<DiscountGroups> Group { get; set; }

        private object _SelectedType;
        public object SelectedType
        {
            get => _SelectedType;
            set => Set(ref _SelectedType, value);
        }

        public ICollection<string> Types { get; } = new List<string>() { "Фиксированная сумма", "Процент"}; 

        /******************************************************/
        public ObservableCollection<DiscountGroups> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public DiscountGroups Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;




        private ObservableCollection<DiscountGroups> _Collection;
        private DiscountGroupsWindow Window;
        private ObservableCollection<DiscountGroups> GetCollection() => db.DiscountGroups.OrderBy(d => d.Name).ToObservableCollection();
        private void CreateNewWindow() => Window = new DiscountGroupsWindow();
        private DiscountGroups CreateNewModel() => new DiscountGroups();

        private DiscountGroups GetModelById(int id)
        {
            return Collection.Where(f => f.Id == id).FirstOrDefault();
        }

        private void Add()
        {
            Model.Guid = Dental.Services.KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
            Collection.Add(Model);
        }
        private void Update()
        {
            if (string.IsNullOrEmpty(Model.Guid)) Dental.Services.KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
            var index = Collection.IndexOf(Model);
            if (index != -1) Collection[index] = Model;
        }

        private void Delete(ObservableCollection<DiscountGroups> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }
    }
}
