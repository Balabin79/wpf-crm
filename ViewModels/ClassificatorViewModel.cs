using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class ClassificatorViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClassificatorViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Прайс услуг\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ExpandTree(object p)
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
        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p == null) return;
                Model = GetModelById((int)p);
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) Delete(new ObservableCollection<Service>() { Model });
                else
                {
                    Delete(new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<Service>().ToObservableCollection());
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        [Command]
        public void Save()
        {
            try
            {
                //ищем совпадающий элемент
                var matchingItem = Collection.Where(f => f.IsDir == Model.IsDir && f.Name == Model.Name && Model.Guid != f.Guid).ToList();

                if (SelectedGroup != null) 
                {
                    int id = ((Service)SelectedGroup).Id;
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
        [Command]
        public void OpenForm(object p)
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
                        Title = "Добавить услугу";
                        Group = Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).ToObservableCollection();
                        VisibleItemForm();
                        break;
                    case -2:
                        Model = CreateNewModel();
                        Title = "Добавить группу";
                        Model.IsDir = 1;
                        Group = Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).ToObservableCollection();
                        if (Group.Count != 0) Group.Add(WithoutCategory);
                        VisibleItemGroup();
                        break;
                    default:
                        Model = GetModelById(param);
                        Group = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                            .GetDirectories().OfType<Service>().ToObservableCollection();
                        if (Group.Count > 0 && Model.ParentId != null && Model.IsDir == 1) Group.Add(WithoutCategory);
                        SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();

                        if (Model.IsDir == 0)
                        {
                            Title = "Редактировать услугу";
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
        [Command]
        public void CancelForm() => Window.Close();

        /************* Специфика этой ViewModel ******************/

        public ObservableCollection<Service> Group
        {
            get { return GetProperty(() => Group); }
            set { SetProperty(() => Group, value); }
        }

        public Service WithoutCategory { get; set; } = new Service() { Id = 0, IsDir = null, ParentId = null, Name = "Без категории" };

        public object SelectedGroup
        {
            get { return GetProperty(() => SelectedGroup); }
            set { SetProperty(() => SelectedGroup, value); }
        }

        /******************************************************/
        public ObservableCollection<Service> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }


        public Service Model { get; set; }
        public string Title { get; set; }
        public Visibility IsVisibleItemForm { get; set; } = Visibility.Hidden;
        public Visibility IsVisibleGroupForm { get; set; } = Visibility.Hidden;


        private void VisibleItemForm()
        {
            IsVisibleItemForm = Visibility.Visible;
            IsVisibleGroupForm = Visibility.Hidden;
            Window.Width = 800;
            Window.Height = 280;
        }
        private void VisibleItemGroup()
        {
            IsVisibleItemForm = Visibility.Hidden;
            IsVisibleGroupForm = Visibility.Visible;
            Window.Width = 800;
            Window.Height = 240;
        }

        private ClassificatorWindow Window;

        private ObservableCollection<Service> GetCollection() => db.Services.OrderBy(d => d.Name).ToObservableCollection();

        private void CreateNewWindow() => Window = new ClassificatorWindow();
        private Service CreateNewModel() => new Service();

        private Service GetModelById(int id) => Collection.Where(f => f.Id == id).FirstOrDefault();     

        private void Add()
        {
            db.Entry(Model).State = EntityState.Added;
            if (db.SaveChanges() > 0) Collection.Add(Model);             
        }
        private void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            if (db.SaveChanges() > 0) Model.UpdateFields();
         
        }

        private void Delete(ObservableCollection<Service> collection)
        {
            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }
    }
}
