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

namespace Dental.ViewModels
{
    class ClassificatorViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClassificatorViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);
            PriceRateForClientsCommand = new LambdaCommand(OnPriceRateForClientsCommandExecuted, CanPriceRateForClientsCommandExecute);
            WageRateForEmploymentsCommand = new LambdaCommand(OnWageRateForEmploymentsCommandExecuted, CanWageRateForEmploymentsCommandExecute);
            CancelWageRateForEmploymentsCommand = new LambdaCommand(OnCancelWageRateForEmploymentsExecuted, CanCancelWageRateForEmploymentsCommandExecute);
           
            
            AddRowInPriceForClientsCommand = new LambdaCommand(OnAddRowInPriceForClientsCommandExecuted, CanAddRowInPriceForClientsCommandExecute);
            DeleteRowInPriceForClientsCommand = new LambdaCommand(OnDeleteRowInPriceForClientsCommandExecuted, CanDeleteRowInPriceForClientsCommandExecute);
            SaveRowInPriceForClientsCommand = new LambdaCommand(OnSaveRowInPriceForClientsCommandExecuted, CanSaveRowInPriceForClientsCommandExecute);

            ExpandTreeCommand = new LambdaCommand(OnExpandTreeCommandExecuted, CanExpandTreeCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
                PriceRateForClients = db.PriceRateForClients.ToList();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Классификация услуг\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand CancelWageRateForEmploymentsCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }
        public ICommand PriceRateForClientsCommand { get; }
        public ICommand WageRateForEmploymentsCommand { get; }   
          
        public ICommand AddRowInPriceForClientsCommand { get; }
        public ICommand DeleteRowInPriceForClientsCommand { get; }
        public ICommand SaveRowInPriceForClientsCommand { get; }


        public ICommand ExpandTreeCommand { get; }


        private bool CanAddRowInPriceForClientsCommandExecute(object p) => true;
        private bool CanDeleteRowInPriceForClientsCommandExecute(object p) => true;
        private bool CanSaveRowInPriceForClientsCommandExecute(object p) => true;
        private bool CanCloseEditorPriceForClientsCommandExecute(object p) => true;

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;

        private bool CanCancelWageRateForEmploymentsCommandExecute(object p) => true;
        private bool CanPriceRateForClientsCommandExecute(object p) => true;
        private bool CanWageRateForEmploymentsCommandExecute(object p) => true;
       
        private bool CanExpandTreeCommandExecute(object p) => true;

        private void OnExpandTreeCommandExecuted(object p)
        {
            try
            {
                if (p is TreeListView tree)
                {
                    foreach (var node in tree.Nodes)
                    {
                        if (node.IsExpanded)
                        {
                            tree.CollapseAllNodes();                           
                            return;
                        }
                    }
                    tree.ExpandAllNodes();
                    return;
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveRowInPriceForClientsCommandExecuted(object p)
        {
            try
            {
                if (p is Classificator collection)
                {
                    if (collection.PriceForClients.Count == 0) return;
                    foreach (var group in collection.PriceForClients.GroupBy(f => f.PriceRateForClientsId))
                    {
                        if (group.Count() > 1)
                        {
                            var response = ThemedMessageBox.Show(title: "Внимание", text: "В прайсе для клиентов имеются задублированные позиции по тарифам! Рекомендуется удалить такие позиции, иначе возможны коллизии при формировании счетов. Чтобы отменить, выберите \"Cancel\", чтобы все равно сохранить - выберите \"Ок\"", messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

                            if (response.ToString() == "Cancel") return;
                        }
                    }
                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteRowInPriceForClientsCommandExecuted(object p)
        {
            try
            {
                if (p is PriceForClients row)
                {
                    Collection.Where(f => f.Id == row.Classificator.Id).FirstOrDefault()?.PriceForClients.Remove(row);
                }
                db.SaveChanges();

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddRowInPriceForClientsCommandExecuted(object p)
        {
            try
            {
                if (p is Classificator row)
                {
                    Collection.Where(f => f.Id == row.Id).FirstOrDefault()?.PriceForClients.Insert(0,
                        new PriceForClients() { Price = "", Classificator = row, ClassificatorId = row.Id, PriceRateForClientsId = 1, PriceRateForClients = db.PriceRateForClients.Where(d => d.Id == 1 ).FirstOrDefault() }
                        );
                }

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnPriceRateForClientsCommandExecuted(object p)
        {
            try
            {
                PriceRateForClientsWindow = new PriceRateForClientsWindow();
                PriceRateForClientsWindow.ShowDialog();
                PriceRateForClients = db.PriceRateForClients.ToList();
                return;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnWageRateForEmploymentsCommandExecuted(object p)
        {
            try
            {
                WageRateForEmploymentsWindow = new WageRateForEmploymentsWindow();
                WageRateForEmploymentsWindow.ShowDialog();
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
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<Classificator>() { Model });
                else Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetItemChilds().OfType<Classificator>().ToObservableCollection());
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

                if (SelectedGroup != null) 
                {
                    int id = ((Classificator)SelectedGroup).Id;
                    if (id == 0) Model.ParentId = null;
                    else Model.ParentId = id;
                } 

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
                        Title = "Новая позиция";
                        Group = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Title = "Создать группу";
                        Model.IsDir = 1;
                        Group = Collection.Where(f => f.IsDir == 1 && f.Id != Model?.Id).OrderBy(f => f.Name).ToObservableCollection();
                        Group.Add(WithoutCategory);
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        Group = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<Classificator>().ToObservableCollection();
                        if (Group.Count > 0 && Model.ParentId != null && Model.IsDir == 1) Group.Add(WithoutCategory);
                        SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();

                        if (Model.IsDir == 0)
                        {
                            Title = "Редактировать позицию";
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
        private void OnCancelWageRateForEmploymentsExecuted(object p) => WageRateForEmploymentsWindow.Close();

        /************* Специфика этой ViewModel ******************/
        private ObservableCollection<Classificator> _Group;
        public ObservableCollection<Classificator> Group
        {
            get => _Group;
            set => Set(ref _Group, value);
        }

        public Classificator WithoutCategory { get; set; } = new Classificator() { Id = 0, IsDir = null, ParentId = null, Name = "Без категории" };

        private object _SelectedGroup;
        public object SelectedGroup
        {
            get => _SelectedGroup;
            set => Set(ref _SelectedGroup, value);
        }

        /******************************************************/
        public ObservableCollection<Classificator> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public Classificator Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Width = 800;
        }
        private void VisibleItemGroup()
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Width = 800;
        }


        public IEnumerable<PriceRateForClients> PriceRateForClients { get; set; }
        public IEnumerable<WageRateForEmployments> WageRateForEmployments { get => db.WageRateForEmployments.ToList(); }

        private ObservableCollection<Classificator> _Collection;
        private WageRateForEmploymentsWindow WageRateForEmploymentsWindow;
        private ClassificatorWindow Window;

        private ObservableCollection<Classificator> GetCollection() => db.Classificator
            .Include(d => d.PriceForClients.Select(f => f.PriceRateForClients))
            .OrderBy(d => d.Name).ToObservableCollection();

        private void CreateNewWindow() => Window = new ClassificatorWindow();
        private Classificator CreateNewModel() => new Classificator();

        private Classificator GetModelById(int id)
        {
            return Collection.Where(f => f.Id == id).FirstOrDefault();
        }

        private void Add()
        {
            Model.Guid = KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Added;
            db.SaveChanges();
            Collection.Add(Model);
        }
        private void Update()
        {
            if (string.IsNullOrEmpty(Model.Guid)) Model.Guid = KeyGenerator.GetUniqueKey();
            db.Entry(Model).State = EntityState.Modified;
            db.SaveChanges();
            var index = Collection.IndexOf(Model);
            if (index != -1) Collection[index] = Model;
        }

        private void Delete(ObservableCollection<Classificator> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }



        public PriceRateForClientsWindow PriceRateForClientsWindow;
    }
}
