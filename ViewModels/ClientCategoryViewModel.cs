using Dental.Infrastructures.Collection;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using Dental.Views.WindowForms;
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

namespace Dental.ViewModels
{
    class ClientCategoryViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientCategoryViewModel()
        {
            try
            {
                db = new ConnectToDb().Context;
                SetCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Категории клиентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

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
                (new ViewModelLog(e)).run();
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
                (new ViewModelLog(e)).run();
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
                (new ViewModelLog(e)).run();
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
