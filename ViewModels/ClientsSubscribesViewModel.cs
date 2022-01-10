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
    class ClientsSubscribesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientsSubscribesViewModel()
        {


            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);
            CancelFormCommand = new LambdaCommand(OnCancelFormCommandExecuted, CanCancelFormCommandExecute);
          

            ExpandTreeCommand = new LambdaCommand(OnExpandTreeCommandExecuted, CanExpandTreeCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Классификация услу\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenFormCommand { get; }
        public ICommand CancelFormCommand { get; }
 
        public ICommand ExpandTreeCommand { get; }



        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;
        private bool CanCancelFormCommandExecute(object p) => true;
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
        private void OnCancelFormCommandExecuted(object p) => Window.Close();
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
                        Group = Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Title = "Создать группу";
                        Model.IsDir = 1;
                        Group = Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).ToObservableCollection();
                        if (Group.Count != 0) Group.Add(WithoutCategory);
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        Group = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<ClientsSubscribes>().ToObservableCollection();
                        if (Group.Count > 0 && Model.ParentId != null && Model.IsDir == 1) Group.Add(WithoutCategory);
                        SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();
                        SelectedClientGroup = ClientsGroups.Where(f => f.Id == Model?.ClientGroupId).FirstOrDefault();
                        SelectedStatus = StatusesSubscribe.Where(f => f.Id == Model?.StatusSubscribeId).FirstOrDefault();
                        SelectedType = TypesSubscribe.Where(f => f.Id == Model?.SubscribeTypeId).FirstOrDefault();

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
                SelectedClientGroup = null;
                SelectedStatus = null;
                SelectedType = null;
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
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Guid != f.Guid).ToList();

                if (SelectedGroup != null)
                {
                    int id = ((ClientsSubscribes)SelectedGroup).Id;
                    if (id == 0) Model.ParentId = null;
                    else Model.ParentId = id;
                }

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    new TryingCreatingDuplicate().run(Model.IsDir);
                    return;
                }
                if (SelectedClientGroup != null) Model.ClientGroupId = ((ClientsGroup)SelectedClientGroup)?.Id;
                if (SelectedStatus != null) Model.StatusSubscribeId = ((StatusSubscribe)SelectedStatus)?.Id;
                if (SelectedType != null) Model.SubscribeTypeId = ((TypeSubscribe)SelectedType)?.Id;

                if (Model.Id == 0) Add(); else Update();
                db.SaveChanges();

                SelectedGroup = null;
                SelectedClientGroup = null;
                SelectedStatus = null;
                SelectedType = null;
                Window.Close();
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

                if (Model.IsDir == 0) Delete(new ObservableCollection<ClientsSubscribes>() { Model });
                else Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetItemChilds().OfType<ClientsSubscribes>().ToObservableCollection());
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }



        

        /************* Специфика этой ViewModel ******************/
        private ObservableCollection<ClientsSubscribes> _Group;
        public ObservableCollection<ClientsSubscribes> Group
        {
            get => _Group;
            set => Set(ref _Group, value);
        }

        public ClientsSubscribes WithoutCategory { get; set; } = new ClientsSubscribes() { Id = 0, IsDir = null, ParentId = null, Name = "Без категории" };

        private object selectedGroup;
        public object SelectedGroup
        {
            get => selectedGroup;
            set => Set(ref selectedGroup, value);
        }

        private object selectedClientGroup;
        public object SelectedClientGroup
        {
            get => selectedClientGroup;
            set => Set(ref selectedClientGroup, value);
        }

        private object selectedStatus;
        public object SelectedStatus
        {
            get => selectedStatus;
            set => Set(ref selectedStatus, value);
        }

        private object selectedType;
        public object SelectedType
        {
            get => selectedType;
            set => Set(ref selectedType, value);
        }

        /******************************************************/
        public ObservableCollection<ClientsSubscribes> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        public ClientsSubscribes Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Width = 800;
            Window.Height = 450;
        }
        private void VisibleItemGroup()
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Width = 800;
            Window.Height = 280;
        }


        private ObservableCollection<ClientsSubscribes> _Collection;
        private ClientsSubscribesWindow Window;

        private ObservableCollection<ClientsSubscribes> GetCollection() => db.ClientsSubscribes.OrderBy(d => d.Name).ToObservableCollection();
        public ICollection<ClientsGroup> ClientsGroups { get => db.ClientsGroup.ToArray(); }
        public ICollection<StatusSubscribe> StatusesSubscribe { get => db.StatusSubscribe.ToArray(); }
        public ICollection<TypeSubscribe> TypesSubscribe { get => db.TypeSubscribe.ToArray(); }
 

        private void CreateNewWindow() => Window = new ClientsSubscribesWindow();
        private ClientsSubscribes CreateNewModel() => new ClientsSubscribes();

        private ClientsSubscribes GetModelById(int id) => Collection.Where(f => f.Id == id).FirstOrDefault();
     

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
            Model.UpdateFields();
        }

        private void Delete(ObservableCollection<ClientsSubscribes> collection)
        {
            Collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            Collection.ForEach(f => Collection.Remove(f));
        }
    }
}
