using System;
using System.Collections.Generic;
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
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Views.WindowForms;
using Dental.Services;
using Dental.Views.Estimates;

namespace Dental.ViewModels
{
    class EstimateViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public EstimateViewModel()
        {
            DeleteEstimateCommand = new LambdaCommand(OnDeleteEstimateCommandExecuted, CanDeleteEstimateCommandExecute);
            SaveEstimateCommand = new LambdaCommand(OnSaveEstimateCommandExecuted, CanSaveEstimateCommandExecute);
            OpenFormEstimateCommand = new LambdaCommand(OnOpenFormEstimateCommandExecuted, CanOpenFormEstimateCommandExecute);
            CancelFormEstimateCommand = new LambdaCommand(OnCancelFormEstimateCommandExecuted, CanCancelFormEstimateCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Сметы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        #region Работа со сметами
        public ICommand DeleteEstimateCommand { get; }
        public ICommand SaveEstimateCommand { get; }
        public ICommand OpenFormEstimateCommand { get; }
        public ICommand CancelFormEstimateCommand { get; }

        private bool CanDeleteEstimateCommandExecute(object p) => true;
        private bool CanSaveEstimateCommandExecute(object p) => true;
        private bool CanOpenFormEstimateCommandExecute(object p) => true;
        private bool CanCancelFormEstimateCommandExecute(object p) => true;

        private void OnOpenFormEstimateCommandExecuted(object p)
        {
            try
            {
                Window = new EstimateWindow();
                Window.DataContext = this;
                Clients = db.Clients.ToList();

                if (p == null || !int.TryParse(p.ToString(), out int param))
                {
                    //Estimate = new Estimate() { EstimateServiseItems = new EstimateServiceItem()};
                    Title = "Новая смета";
                }
                else
                {
                   // Estimate = db.Estimates.Where(f => f.Id == param).Include(f => f.EstimateServiceItems).FirstOrDefault();
                    Title = "Редактирование сметы №" + param;
                }                      
                
                Window.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveEstimateCommandExecuted(object p)
        {
            try
            {
                // Если нет корневой директории (ФИО клиента, то создаем ее)               
               
                
                db.SaveChanges();


                Window.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private int CreateRootDir()
        {
           // var model = new EstimateServiceItems() { Name = Estimate.EstimateServiceItems.Client.FullName };
            //db.EstimateServiceItems.Add(model);
            db.SaveChanges();
            //return model.Id;
            return 0;
        }

        private void OnDeleteEstimateCommandExecuted(object p)
        {
            try
            {/*
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<Estimate>() { Model });
                else
                {
                    Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<Estimate>().ToObservableCollection());
                    ActionsLog.RegisterAction(Model.Name, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["Estimates"]);
                }
                db.SaveChanges();*/
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }


        private void OnCancelFormEstimateCommandExecuted(object p) => Window.Close();
        #endregion

        public ICollection<Client> Clients { get; set; }
        public Estimate Estimate { get; set; }
        public EstimateServiceItem EstimateCategory { get; set; }
        public string Title { get; set; }











        /************* Специфика этой ViewModel ******************/
        private ObservableCollection<Estimate> _Group;
        public ObservableCollection<Estimate> Group
        {
            get => _Group;
            set => Set(ref _Group, value);
        }

        public Estimate WithoutCategory { get; set; } = new Estimate() { Id = 0,  Name = "Без категории" };

        private object _SelectedGroup;
        public object SelectedGroup
        {
            get => _SelectedGroup;
            set => Set(ref _SelectedGroup, value);
        }

        /******************************************************/
        public ObservableCollection<EstimateServiceItem> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }


        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Width = 800;
            Window.Height = 328;
        }
        private void VisibleItemGroup()
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Width = 800;
            Window.Height = 280;
        }

        private ObservableCollection<EstimateServiceItem> _Collection;
        private EstimateWindow Window;

        private ObservableCollection<EstimateServiceItem> GetCollection() 
        {
           // db.EstimateServiceItems.Include(f => f.Estimates).Include(f => f.Client).OrderBy(d => d.Name).Load();
            return db.EstimateServiceItems.Local;

        }     
    }
}

