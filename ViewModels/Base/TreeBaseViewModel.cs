using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using B6CRM.Models.Base;
using System.Collections.ObjectModel;
using B6CRM.Infrastructures.Collection;
using B6CRM.Views.ServicePrice;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Infrastructures.Converters;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Models.Templates;

namespace B6CRM.ViewModels.Base
{
    public class TreeBaseViewModel<T> : ViewModelBase where T : BaseTemplate<T>
    {
        protected ApplicationContext db;
        public TreeBaseViewModel(ApplicationContext ctx, DbSet<T> context)
        {
            db = ctx;
            Context = context;
            Collection = GetCollection();
        }

        public bool CanAdd(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PriceEditable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).PriceEditable;
        public bool CanMove(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PriceEditable;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PriceDelitable;

        [Command]
        public void Add(object p)
        {
            try
            {
                if (p is CommandParameters parameters)
                {
                    if (parameters.Row is T row && int.TryParse(parameters.Type.ToString(), out int result))
                    {
                        var model = (T)Activator.CreateInstance(typeof(T));
                        model.IsDir = result;
                        model.ParentID = row?.Id == 0 ? null : row?.Id;
                        model.Parent = row?.Id == 0 ? null : row;
                        db.Entry(model).State = EntityState.Added;
                        db.SaveChanges();

                        Collection.Add(model);
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При добавлении элемента произошла ошибка!", true);
            }
        }

        [Command]
        public void Save()
        {
            try
            {
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке сохранения в базу данных произошла ошибка!", true);
            }
        }

        [Command]
        public void Move(object p)
        {
            try
            {
                if (p is RoutedEventArgs args && p is DropRecordEventArgs target)
                {
                    /* object data = target.Data.GetData(typeof(RecordDragDropData));
                     foreach (T model in ((RecordDragDropData)data).Records)
                     {
                         //model.Parent = (T)target.TargetRecord;
                         model.ParentID = ((T)target.TargetRecord).ParentID;
                     }*/




                    //var parent = target.TargetRecord as T;
                    //model.Parent = parent;
                    //model.ParentId = parent?.Id;
                    //args.Handled = true;                                   
                }
                //if (db.SaveChanges() > 0) new Notification() { Content = "Изменения  сохранены в базу данных!" }.run();
                //Collection = GetCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке перемещения элемента и его сохранения в базу данных произошла ошибка!", true);
            }
        }

        [AsyncCommand]
        public async Task Delete(object p)
        {
            try
            {
                if (p is T model && new ConfirDeleteInCollection().run(model.IsDir))
                {
                    if (model.IsDir == 0)
                    {
                        db.Entry(model).State = EntityState.Deleted;
                        if (db.SaveChanges() > 0) new Notification() { Content = "Позиция удалена из базы данных!" }.run();
                        Collection.Remove(model);
                    }
                    else
                    {
                        DelItems = new List<T>();
                        await Delete(model);
                        if (db.SaveChanges() > 0) new Notification() { Content = "Директория удалена из базы данных!" }.run();
                        DelItems?.ForEach(f => Collection.Remove(f));
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке удаления произошла ошибка!", true);
            }
        }

        private List<T> DelItems { get; set; }

        private async Task Delete(T model)
        {
            if (model?.IsDir == 1)
            {
                var nodes = Context.Where(f => f.ParentID == model.Id).ToArray();
                foreach (var node in nodes)
                {
                    if (node.IsDir == 1) await Delete(node);
                    db.Entry(node).State = EntityState.Deleted;
                    DelItems.Add(node);
                }
            }

            db.Entry(model).State = EntityState.Deleted;
            DelItems.Add(model);
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
                Log.ErrorHandler(e);
            }
        }

        public virtual ObservableCollection<T> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public DbSet<T> Context { get; set; }

        protected virtual ObservableCollection<T> GetDirs(T model) => Context?.OfType<T>().Where(f => f.IsDir == 1 && f.Id != model.Id).ToObservableCollection();

        protected virtual ObservableCollection<T> GetCollection() => Context?.Include(f => f.Parent).OrderByDescending(f => f.IsDir).OrderBy(f => f.Name).ToObservableCollection();



    }
}
