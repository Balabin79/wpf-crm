using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels
{
    class AdvertisingViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;

        public AdvertisingViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
                
                Collection.ForEach(f => CollectionBeforeChanges.Add((Advertising)f.Clone()));
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Рекламные источники\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is Advertising model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0) 
                    {
                        db.Entry(model).State = EntityState.Deleted;
                        db.SaveChanges();
                        if (db.SaveChanges() > 0) new Notification() { Content = "Успешно удалено из базы данных!" }.run();
                        
                    } 
                    else db.Entry(model).State = EntityState.Detached;
                    Collection.Remove(model);
                    CollectionBeforeChanges.Clear();
                    Collection.ForEach(f => CollectionBeforeChanges.Add((Advertising)f.Clone()));
                }
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
                foreach (var item in Collection)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                Collection = GetCollection();
                CollectionBeforeChanges.Clear();
                Collection.ForEach(f => CollectionBeforeChanges.Add((Advertising)f.Clone()));
                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void Add() => Collection.Add(new Advertising());


        public ObservableCollection<Advertising> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        private ObservableCollection<Advertising> GetCollection() => db.Advertising?.OrderBy(d => d.Name).ToObservableCollection();
        public ObservableCollection<Advertising> CollectionBeforeChanges { get; set; } = new ObservableCollection<Advertising>();

        public bool HasUnsavedChanges()
        {
            if (CollectionBeforeChanges?.Count != Collection.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Name)) continue;
                if (item.Id == 0) return true;
                if (!item.Equals(CollectionBeforeChanges.Where(f => f.Guid == item.Guid).FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning) ;

            return response.ToString() == "No";
        }
    }
}
