using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using B6CRM.Infrastructures.Extensions.Notifications;
using System;
using B6CRM.Models;
using B6CRM.Services;

namespace B6CRM.ViewModels.AdditionalFields
{
    public class AdditionalClientFieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public delegate void FieldChanges();
        public event FieldChanges EventFieldChanges;

        //это ViewModel управляет справочником "Дополнительные поля" вызывается как отдельное окно
        public AdditionalClientFieldsViewModel(ApplicationContext ctx)
        {
            try
            {
                db = ctx;
                SetCollection();
                Templates = db.TemplateType.ToArray();
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
                if (p is AdditionalClientField model)
                {
                    if (model.Id != 0 && !DeleteMsgShow(model)) return;

                    if (model.Id != 0)
                    {
                        db.AdditionalClientValue.Where(f => f.AdditionalFieldId == model.Id)?.ForEach(f => db.AdditionalClientValue.Remove(f));
                        db.AdditionalClientFields.Remove(model);

                        if (db.SaveChanges() > 0)
                        {
                            EventFieldChanges?.Invoke();
                            new Notification() { Content = "Успешно удалено из базы данных!" }.run();
                        }
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Detached;
                    }
                    Collection?.Remove(model);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке удаления произошла ошибка!", true);
            }
        }

        private bool DeleteMsgShow(AdditionalClientField model)
        {

            var val = db.AdditionalClientValue.Where(f => f.AdditionalFieldId == model.Id).Count();
            string msg = val < 1 ? "Вы уверены?" : "Внимание! В поле " + model.Label + " записано значение, которое заполнено в картах клиентов (Всего: " + val + "). Вы уверены что хотите удалить это поле?";
            var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: msg,
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);

            return response.ToString() == "Yes";

        }

        [Command]
        public void Save()
        {
            try
            {
                foreach (var item in Collection)
                {
                    if (string.IsNullOrEmpty(item.Label) || string.IsNullOrEmpty(item.SysName)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                if (db.SaveChanges() > 0)
                {
                    SetCollection();
                    EventFieldChanges?.Invoke();
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке сохранения изменений в базу данных, произошла ошибка!", true);
            }
        }

        [Command]
        public void Add() => Collection.Add(new AdditionalClientField());

        private void SetCollection() => Collection = db.AdditionalClientFields.OrderBy(f => f.Label).Include(f => f.TypeValue).ToObservableCollection() ?? new ObservableCollection<AdditionalClientField>();


        public bool HasUnsavedChanges()
        {

            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Label) || string.IsNullOrEmpty(item.SysName)) continue;
                if (item.Id == 0) return true;

            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            return response.ToString() == "No";
        }

        public ICollection<TemplateType> Templates { get; private set; }

        public ObservableCollection<AdditionalClientField> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }
    }
}
