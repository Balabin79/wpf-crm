using System;
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
using Dental.Interfaces;
using Dental.Models.Template;
using Dental.Views.Nomenclatures;

namespace Dental.ViewModels
{
    class CommunicationLogViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public CommunicationLogViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            OpenThemaFormCommand = new LambdaCommand(OnOpenThemaFormCommandExecuted, CanOpenThemaFormCommandExecute);
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
        public ICommand OpenThemaFormCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanOpenThemaFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<CommunicationLog>() { Model });
                else Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetItemChilds().OfType<CommunicationLog>().ToObservableCollection());
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
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Id != f.Id).ToList();

                if (SelectedGroup != null) Model.ParentId = ((Role)SelectedGroup).Id;

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    new TryingCreatingDuplicate().run(Model.IsDir);
                    return;
                }

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();

                SelectedGroup = null;
                Window.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenThemaFormCommandExecuted(object p)
        {
            CommunicationLogThemasWindow = new CommunicationLogThemas();
            CommunicationLogThemasWindow.DataContext = this;
            CommunicationLogThemasWindow.ShowDialog();
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
                        Model.IsDir = 0;
                        Title = "Создать позицию";
                        Group = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Title = "Создать группу";
                        Model.IsDir = 1;
                        Group = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        Group = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<CommunicationLog>().ToObservableCollection();

                        SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();

                        if (Model.IsDir == 0)
                        {
                            Title = "Редактировать роль";
                            VisibleItemForm();
                        }
                        else
                        {
                            Title = "Редактировать группу";
                            VisibleItemGroup();
                        }
                        break;
                }

                Window.DataContext = this;
                Window.ShowDialog();
                SelectedGroup = null;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnCancelFormCommandExecuted(object p) => Window.Close();

        /************* Специфика этой ViewModel ******************/
        public ICollection<CommunicationLog> Group { get; set; }

        private object _SelectedGroup;
        public object SelectedGroup
        {
            get => _SelectedGroup;
            set => Set(ref _SelectedGroup, value);
        }

        /******************************************************/

        public CommunicationLog Model { get; set; }
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

 
        private CommunicationLogWindow Window;
        private CommunicationLogThemas CommunicationLogThemasWindow;


        private void CreateNewWindow() => Window = new CommunicationLogWindow();
        private CommunicationLog CreateNewModel() => new CommunicationLog();

        private CommunicationLog GetModelById(int id)
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

        private void Delete(ObservableCollection<CommunicationLog> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }

        public ObservableCollection<CommunicationLog> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        /*
                public ObservableCollection<CommunicationLog> Communications
                {
                    get => _Communications;
                    set => Set(ref _Communications, value);
                }
        */
        private ObservableCollection<CommunicationLog> GetCollection() => db.CommunicationLog.OrderBy(d => d.Name).ToObservableCollection();

        public IEnumerable<CommunicationLogThema> Themas { get => db.CommunicationLogThema.OrderBy(f => f.Name).ToList(); }
        public IEnumerable<CommunicationLogStatus> Statuses { get => db.CommunicationLogStatus.OrderBy(f => f.Name).ToList(); }
        public IEnumerable<ClientsCategory> ClientsCategories { get => db.ClientsCategory.OrderBy(f => f.Name).ToList(); }
        public IEnumerable<Employee> Employees { get => db.Employes.OrderBy(f => f.LastName).ToList(); }

        private ObservableCollection<CommunicationLog> _Collection;
        private ObservableCollection<CommunicationLog> _Communications;
    }
}
