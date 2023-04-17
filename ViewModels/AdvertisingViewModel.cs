﻿using B6CRM.Infrastructures.Collection;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.ViewModels.ClientDir;

namespace B6CRM.ViewModels
{
    class AdvertisingViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public delegate void AdvertisingChanges();
        public event AdvertisingChanges EventAdvertisingChanges;

        public AdvertisingViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Рекламные источники\"!", true);
            }
        }

        public bool CanOpenWindow(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsAdvertisingEditable;

        [Command]
        public void OpenWindow(object p)
        {
            try
            {
                db.Advertising?.ForEach(f => db.Entry(f).State = EntityState.Unchanged);
                SetCollection();
                if (p is ClientsViewModel vm && vm != null) 
                    EventAdvertisingChanges += vm.AdvertisingLoad;
                
                new AdvertisingsWindow() { DataContext = this }.Show();
                //StatusWindow.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Add(object p) => Collection?.Add(new Advertising());

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
                    EventAdvertisingChanges?.Invoke();
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
                if (p is Advertising model)
                {
                    if (model.Id != 0)
                    {
                        if (!new ConfirDeleteInCollection().run(0)) return;
                        db.Entry(model).State = EntityState.Deleted;
                    }

                    else db.Entry(model).State = EntityState.Detached;

                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Рекламный источник удален из базы данных!" }.run();
                        SetCollection();
                        EventAdvertisingChanges?.Invoke();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public ObservableCollection<Advertising> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        private void SetCollection() => Collection = db.Advertising.OrderBy(f => f.Name).ToObservableCollection();
    }
}
