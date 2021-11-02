using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class WageRateViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public WageRateViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
            OpenFormCommand = new LambdaCommand(OnOpenFormCommandExecuted, CanOpenFormCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
                Collection.ForEach(f => CollectionBeforeChanges.Add((WageRateForEmployments)f.Clone()));
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Тарифы для клиентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand OpenFormCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;
        private bool CanOpenFormCommandExecute(object p) => true;

        private void OnOpenFormCommandExecuted(object p)
        {
            try
            {
                Window = new WageRateForEmploymentsWindow();
                GetCollection();
                Window.DataContext = this;
                Window.ShowDialog();
                return;
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p is WageRateForEmployments model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;

                    Collection.Remove(model);
                    if (model.Id != 0) db.Entry(model).State = EntityState.Deleted;
                    else db.Entry(model).State = EntityState.Detached;
                    db.SaveChanges();
                    CollectionBeforeChanges = new ObservableCollection<WageRateForEmployments>();
                    Collection.ForEach(f => CollectionBeforeChanges.Add((WageRateForEmployments)f.Clone()));
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSaveCommandExecuted(object p)
        {
            try
            {
                foreach (var item in Collection)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;

                    if (item.Id == 0)
                    {
                        item.Guid = KeyGenerator.GetUniqueKey();
                        db.Entry(item).State = EntityState.Added;
                        continue;
                    }
                    else if (string.IsNullOrEmpty(item.Guid)) item.Guid = KeyGenerator.GetUniqueKey();
                    if (item.IsApplyRule == 0) item.MoreOrLess = item.PercentOrCost = item.Value = null;
                }
                int rows = db.SaveChanges();
                Collection.Where(f => f.Id == 0).ToArray().ForEach(f => Collection.Remove(f));

                CollectionBeforeChanges = new ObservableCollection<WageRateForEmployments>();
                Collection.ForEach(f => CollectionBeforeChanges.Add((WageRateForEmployments)f.Clone()));
                if (rows != 0)
                {
                    var notification = new Notification();
                    notification.Content = "Изменения сохранены в базу данных!";
                    notification.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnAddCommandExecuted(object p) => Collection.Add(new WageRateForEmployments());

        public ObservableCollection<WageRateForEmployments> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<WageRateForEmployments> _Collection;
        private ObservableCollection<WageRateForEmployments> GetCollection() => db.WageRateForEmployments.OrderBy(d => d.Name).ToObservableCollection();
        public ObservableCollection<WageRateForEmployments> CollectionBeforeChanges { get; set; } = new ObservableCollection<WageRateForEmployments>();

        public WageRateForEmploymentsWindow Window;

        public bool HasUnsavedChanges()
        {
            if (CollectionBeforeChanges?.Count != Collection.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Name)) continue;
                if (item.Id == 0) return true;
                if (!item.Equals(
                    CollectionBeforeChanges.
                    Where(
                        f => f.Id == item.Id)
                    .FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {

            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Если хотите сохранить эти данные, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой). Для продолжения без сохранения, нажмите \"Ок\"",
               messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

            return response.ToString() == "Cancel";
        }
        //public bool IsReadOnly { get; set; }
        public List<string> MoreOrLessList { get; set; } = new List<string>() { "Больше", "Меньше" };
        public List<string> PercentOrCostList { get; set; } = new List<string>() { "на процент", "на сумму" };
    }
}
