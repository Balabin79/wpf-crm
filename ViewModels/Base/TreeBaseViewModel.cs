﻿using Dental.Infrastructures.Converters;
using Dental.Models.Templates;
using Dental.Views.Templates;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Models;
using DevExpress.Xpf.Grid;
using Dental.Models.Base;
using System.Collections.ObjectModel;
using Dental.Infrastructures.Collection;
using Dental.Views.ServicePrice;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels.Base
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
                        model.ParentId = row?.Id;
                        model.Parent = row;
                        db.Entry(model).State = EntityState.Added;
                        db.SaveChanges();

                        Collection.Add(model);
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "При добавлении элемента произошла ошибка!",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error
                );
            }
        }

        [Command]
        public void Save()
        {
            try
            {
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения в прайсе сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "При попытке сохранения в базу данных произошла ошибка!",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error
                );
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
                        db.SaveChanges();
                        Collection.Remove(model);
                    }
                    else
                    {
                        DelItems = new List<T>();
                        await Delete(model);
                        db.SaveChanges();                       
                        DelItems?.ForEach(f => Collection.Remove(f));
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке удаления произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private List<T> DelItems { get; set; }

        private async Task Delete(T model)
        {
            if (model?.IsDir == 1)
            {
                var nodes = Context.Where(f => f.ParentId == model.Id).ToArray();
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
            catch { }
        }

        public virtual ObservableCollection<T> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public DbSet<T> Context { get; set; }

        protected virtual ObservableCollection<T> GetDirs(T model) => Context?.OfType<T>().Where(f => f.IsDir == 1 && f.Id != model.Id).ToObservableCollection();

        protected virtual ObservableCollection<T> GetCollection() => Context?.Include(f => f.Parent).OrderByDescending(f => f.IsDir).OrderBy(f => f.Name).ToObservableCollection();

        protected virtual Window GetWindow() => new TemplateWin();


    }
}
