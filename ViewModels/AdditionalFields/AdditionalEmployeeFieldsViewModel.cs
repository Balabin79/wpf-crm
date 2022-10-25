using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.AdditionalFields;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;

namespace Dental.ViewModels.AdditionalFields
{
    public class AdditionalEmployeeFieldsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public AdditionalEmployeeFieldsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
                Templates = db.TemplateType.ToArray();
            }
            catch
            {

            }
        }

        public bool CanDelete(object p) => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeAddFieldsEditable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeAddFieldsEditable;
        public bool CanAdd() => ((UserSession)Application.Current.Resources["UserSession"]).EmployeeAddFieldsEditable;

        [Command]
        public void Delete(object p)
        {
            try
            {
                if (p is AdditionalEmployeeField model)
                {
                    if (model.Id != 0 && !DeleteMsgShow(model)) return;
                    if (model.Id != 0)
                    {
                        db.AdditionalEmployeeValue.Where(f => f.AdditionalFieldId == model.Id)?.ForEach(f => db.AdditionalEmployeeValue.Remove(f));
                        db.AdditionalEmployeeFields.Remove(model);
                        if (db.SaveChanges() > 0) new Notification() { Content = "Успешно удалено из базы данных!" }.run();
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
                ThemedMessageBox.Show(title: "Ошибка", text: "При попытке удаления произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private bool DeleteMsgShow(AdditionalEmployeeField model)
        {
            var val = db.AdditionalEmployeeValue.Where(f => f.AdditionalFieldId == model.Id).Count();
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
                    new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void Add() => Collection.Add(new AdditionalEmployeeField());

        private void SetCollection() => Collection = db.AdditionalEmployeeFields.OrderBy(f => f.Label).Include(f => f.TypeValue).ToObservableCollection() ?? new ObservableCollection<AdditionalEmployeeField>();

        public bool HasUnsavedChanges()
        {
            //  if (CollectionBeforeChanges?.Count != MaterialViewModel?.Measures?.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Label) || string.IsNullOrEmpty(item.SysName)) continue;
                if (item.Id == 0) return true;
                //if (!item.Equals(CollectionBeforeChanges.Where(f => f.Guid == item.Guid).FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            return response.ToString() == "No";
        }


        public AdditionalEmployeeField Model { get; private set; }
        public ICollection<TemplateType> Templates { get; private set; }

        public ObservableCollection<AdditionalEmployeeField> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }
    }
}
