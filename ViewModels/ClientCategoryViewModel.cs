using B6CRM.Infrastructures.Collection;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models.Base;
using B6CRM.Views.WindowForms;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using B6CRM.Models;
using B6CRM.Services;

namespace B6CRM.ViewModels
{
    class ClientCategoryViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientCategoryViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Категории клиентов\"!", true);
            }
        }

        public bool CanOpenWClientCategoryWindow() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsCategoryEditable;

        [Command]
        public void OpenWClientCategoryWindow()
        {
            try
            {
                db.ClientCategories?.ForEach(f => db.Entry(f).State = EntityState.Unchanged);
                SetCollection();
                new ClientCategoriesWindow() { DataContext = this }.Show();
                //StatusWindow.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Add(object p) => Collection?.Add(new ClientCategory());

        [Command]
        public void Save()
        {
            try
            {
                foreach (var item in Collection)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;

                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }

                if (db.SaveChanges() > 0)
                {
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    SetCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is ClientCategory model)
                {
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                    }

                    else db.Entry(model).State = EntityState.Detached;

                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Категория удалена из базы данных!" }.run();
                        SetCollection();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public ObservableCollection<ClientCategory> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        private void SetCollection() => Collection = db.ClientCategories.OrderBy(f => f.Name).ToObservableCollection();
    }
}
