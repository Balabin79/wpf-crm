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
using B6CRM.ViewModels.ClientDir;

namespace B6CRM.ViewModels
{
    class ClientCategoryViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public delegate void ClientCategoryChanges();
        public event ClientCategoryChanges EventClientCategoriesChanges;

        private ClientsViewModel ClientsViewModel { get; set; }
        
        public ClientCategoryViewModel()
        {
            try
            {
                db = new ApplicationContext();
                db.ClientCategories?.ForEach(f => db.Entry(f).State = EntityState.Unchanged);
                SetCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Категории клиентов\"!", true);
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
                    EventClientCategoriesChanges?.Invoke();
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

                        EventClientCategoriesChanges?.Invoke();

                        var clients = db.Clients.Where(f => f.ClientCategoryId == model.Id)?.ToArray();

                        foreach(var i in clients?.ToArray())
                        {
                            i.ClientCategory = null;
                            db.Update(i);
                        }
                        db.SaveChanges();
                        //EventClientCategoriesChanges?.Invoke();

                        //ClientsViewModel.ClientInfoViewModel.ClientCategory = null;
                       // ClientsViewModel.Model.ClientCategory = null;
                        

                        db.Entry(model).State = EntityState.Deleted;
                    }

                    else db.Entry(model).State = EntityState.Detached;

                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Категория удалена из базы данных!" }.run();
                        SetCollection();
                        EventClientCategoriesChanges?.Invoke();
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

        private void SetCollection() => Collection = db.ClientCategories.OrderBy(f => f.Name)?.ToArray()?.ToObservableCollection();
    }
}
