using System;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Collections.Generic;
using Dental.Views.Settings;

namespace Dental.ViewModels.Org
{
    public class CommonValueViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public CommonValueViewModel()
        {
            try
            {
                db = new ApplicationContext();
                CommonValues = db.CommonValues.ToObservableCollection() ?? new ObservableCollection<CommonValue>();

            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Дополнительные значения\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanAdd() => ((UserSession)Application.Current.Resources["UserSession"]).OrgEditable;
        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).OrgEditable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).OrgEditable;

        [Command]
        public void Add() => CommonValues?.Add(new CommonValue());

        [Command]
        public void Save()
        {
            try
            {

                foreach (var item in CommonValues)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;
                }
                if (db.SaveChanges() > 0)
                {
                    CommonValues = db.CommonValues.ToObservableCollection() ?? new ObservableCollection<CommonValue>();
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
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
                if (p is CommonValue model)
                {
                    if (model.Id != 0 && !DeleteMsgShow(model)) return;

                    if (model.Id != 0)
                    {
                        db.Entry(model).State = EntityState.Deleted;
                        if (db.SaveChanges() > 0) new Notification() { Content = "Успешно удалено из базы данных!" }.run();
                    }
                    else
                    {
                        db.Entry(model).State = EntityState.Detached;
                    }

                    CommonValues?.Remove(model);
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private bool DeleteMsgShow(CommonValue model)
        {

            var val = db.CommonValues.FirstOrDefault(f => f.Id == model.Id);
            string msg = val == null ? "Вы уверены?" : "Внимание! В поле " + model.Name + " записано значение. Вы уверены что хотите удалить это поле?";

            var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: msg,
                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
            return response.ToString() == "Yes";

        }

        public bool HasChanges()
        {
            foreach (var item in CommonValues)
            {
                if (string.IsNullOrEmpty(item?.Name)) continue;
                if (item?.Id == 0) return true;
            }
            return false;
        }

        public ICollection<CommonValue> CommonValues
        {
            get { return GetProperty(() => CommonValues); }
            set { SetProperty(() => CommonValues, value); }
        }
    }
}
