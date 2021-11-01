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
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Services;

namespace Dental.ViewModels
{
    class PriceRateForClientsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public PriceRateForClientsViewModel()
        {
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);

            try
            {
                db = new ApplicationContext();
                GetCollection();  
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Тарифы для клиентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;


        private void OnOpenFormCommandExecuted(object p)
        {
            try
            {
                Window = new PriceRateForClientsWindow();
                GetCollection();
                Window.DataContext = this;
                Window.ShowDialog();
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
                
                foreach(var item in Collection)
                {
                    if (item.Id == 0) Add(item);
                    else if (string.IsNullOrEmpty(item.Guid)) item.Guid = Dental.Services.KeyGenerator.GetUniqueKey();
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddCommandExecuted(object p)
        {
            try
            {
                Collection.Insert(0, new PriceRateForClients());
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }


        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(0)) return;
                Delete(new ObservableCollection<PriceRateForClients>() { Model });
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }



     

        private void OnCancelFormCommandExecuted(object p) => Window.Close();


        public ObservableCollection<PriceRateForClients> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<PriceRateForClients> _Collection = new ObservableCollection<PriceRateForClients>();

        public PriceRateForClients Model { get; set; }
        public string Title { get; set; }

        public PriceRateForClientsWindow Window;

        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
        }

        private void GetCollection() 
        {
            db.PriceRateForClients.OrderBy(d => d.Name).Load();
            Collection = db.PriceRateForClients.Local.ToObservableCollection();
        } 


        private PriceRateForClients CreateNewModel() => new PriceRateForClients();

        private PriceRateForClients GetModelById(int id)
        {
            return Collection.Where(f => f.Id == id).FirstOrDefault();
        }

        private void Add(PriceRateForClients item)
        {
            item.Guid = Dental.Services.KeyGenerator.GetUniqueKey();
            db.Entry(item).State = EntityState.Added;
            db.SaveChanges();
        }
        private void Update()
        {
            if (string.IsNullOrEmpty(Model.Guid)) Model.Guid = Dental.Services.KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
            var index = Collection.IndexOf(Model);
            if (index != -1) Collection[index] = Model;
        }

        private void Delete(ObservableCollection<PriceRateForClients> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }
    }
}
