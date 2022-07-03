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
using Dental.Views.ServicePrice;
using Dental.ViewModels.Invoices;
using Dental.Infrastructures.Converters;

namespace Dental.ViewModels.ServicePrice
{
    class ServiceViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public ServiceViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.Services.OrderBy(f => f.IsDir == 0).ThenBy(f => f.Name).Include(f => f.Parent).ToObservableCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Прайс услуг\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void SelectItemInServiceField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Service service)
                    {
                        //if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {

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
            catch {}
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
                var matchingItem = Collection.Where(f => f.IsDir == ServiceVM.IsDir && f.Name == ServiceVM.Name && ServiceVM.Code == f.Code && ServiceVM.Guid != f.Guid).ToList();

                if (matchingItem.Count() > 0 && matchingItem.Any(f => f.ParentId == Model.ParentId))
                {
                    if (!new TryingCreatingDuplicate().run(Model.IsDir)) return;                   
                }

                Model.IsDir = ServiceVM.IsDir;
                Model.Name = ServiceVM.Name;
                Model.Code = ServiceVM.Code;
                Model.Price = ServiceVM.Price;
                Model.Parent = ServiceVM.Parent?.Guid == "000" ? null : ServiceVM.Parent;
                Model.ParentId = ServiceVM.Parent?.Guid == "000" ? null : ServiceVM.ParentId;

                if (Model.Id == 0) 
                {
                    db.Entry(Model).State = EntityState.Added;
                    if (db.SaveChanges() > 0) Collection.Add(Model);
                }  
                else 
                {
                    db.Entry(Model).State = EntityState.Modified;
                    if(db.SaveChanges() > 0)
                    {
                        Collection.Remove(Collection.FirstOrDefault(f => f.Id == Model.Id));
                        Collection.Add(Model);
                    }
                } 
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
                Model = (param > 0) ?  db.Services.Where(f => f.Id == param).Include(f => f.Parent).FirstOrDefault() : new Service();
                Model.IsDir = (param < 0) ? param == -1 ? 0 : 1 : Model.IsDir;
                ServiceVM = new ServiceVM(db, Model.Id)
                {
                    Name = Model.Name,
                    Price = Model.Price,
                    ParentId = Model.ParentId,
                    Parent = Model.Parent,
                    IsDir = Model.IsDir ?? 0,
                    Code = Model.Code,
                    IsVisibleItemForm = Model.IsDir == 0,
                    Guid = Model.Guid
                };

                if ((Model.Id > 0 && ServiceVM.ParentId != null && ServiceVM.Services.Count > 0) || (Model.Id == 0)) ServiceVM.Services?.Add(WithoutCategory);
  
                Window = new ServiceWindow() { DataContext = this, Height = ServiceVM.IsDir == 0 ? 280 : 235};
                Window.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке открытия формы произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
        [Command]
        public void CancelForm() => Window.Close();

        public Service WithoutCategory { get; set; } = new Service() { Id = 0, IsDir = null, ParentId = null, Name = "Без категории", Guid = "000" };
        public ObservableCollection<Service> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }
        public Service Model { get; set; }        
        private ServiceWindow Window;
        public ServiceVM ServiceVM
        {
            get { return GetProperty(() => ServiceVM); }
            set { SetProperty(() => ServiceVM, value); }
        }
    }
}
