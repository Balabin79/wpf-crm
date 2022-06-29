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
                Collection = db.Services.OrderBy(d => d.Name).ToObservableCollection();
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
            catch
            {
                
            }
        }
        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p == null) return;
                Model = Collection.Where(f => f.Id == (int)p).FirstOrDefault();
                if (Model == null || !new ConfirDeleteInCollection().run(Model.IsDir)) return;

                if (Model.IsDir == 0) 
                { 
                    db.Entry(Model).State = EntityState.Deleted;
                    Collection.Remove(Model);
                }
                else
                {
                    var collection = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model).GetItemChilds().OfType<Service>().ToObservableCollection();
                    collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
                    collection.ForEach(f => Collection.Remove(f));
                }

                db.SaveChanges();
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке удаления произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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

                if (Model.Id == 0) 
                {
                    db.Entry(Model).State = EntityState.Added;
                    if (db.SaveChanges() > 0) Collection.Add(Model);
                }  
                else 
                {
                    db.Entry(Model).State = EntityState.Modified;
                    if (db.SaveChanges() > 0) Model.UpdateFields();
                } 
                db.SaveChanges();

                SelectedGroup = null;
                Window.Close();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке сохранения в бд произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        [Command]
        public void OpenForm(object p)
        {
            try
            {
                if (!int.TryParse(p.ToString(), out int param)) return;
                Window = new ClassificatorWindow();
                Model = (param > 0) ?  Collection.Where(f => f.Id == param).FirstOrDefault() : new Service();
                Group = new RecursionByCollection(Collection.OfType<ITreeModel>().ToObservableCollection(), Model)
                        .GetDirectories().OfType<Service>().ToObservableCollection();
                
                if (param > 0) // открываем на редактирование
                {                                                         
                    if (Group.Count > 0 && Model.ParentId != null) Group.Add(WithoutCategory);                  
                    SelectedGroup = Collection.Where(f => f.Id == Model?.ParentId && f.Id != Model.Id).FirstOrDefault();

                    if (Model.IsDir == 0) 
                    {
                        Window.Height = 280;
                        Title = "Редактировать услугу";
                        IsVisibleItemForm = true;
                    } 
                    else 
                    {
                        Window.Height = 240;
                        Title = "Редактировать группу";
                        IsVisibleItemForm = false;
                    } 

                }
                else // открываем на создание
                {
                    if (Collection.Where(f => f.IsDir == 1 && f.Guid != Model?.Guid).OrderBy(f => f.Name).Count() > 0) Group.Add(WithoutCategory);

                    if (param == -1)
                    {
                        Model.IsDir = 0;
                        Window.Height = 280;
                        Title = "Добавить услугу";
                        IsVisibleItemForm = true;
                    }
                    if (param == -2)
                    {
                        Model.IsDir = 1;
                        Window.Height = 240;
                        Title = "Добавить группу";
                        IsVisibleItemForm = false;
                    }
                }

                Window.DataContext = this;
                Window.ShowDialog();
                SelectedGroup = null;
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        [Command]
        public void CancelForm() => Window.Close();



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
        public ObservableCollection<Service> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }
        public Service Model { get; set; }
        public string Title { get; set; }
        public bool IsVisibleItemForm { get; set; }
        private ClassificatorWindow Window;
    }
}
