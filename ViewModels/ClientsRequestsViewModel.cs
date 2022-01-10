using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    class ClientsRequestsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientsRequestsViewModel()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);

            try
            {
                db = new ApplicationContext();
                Collection = GetCollection();
                Collection.ForEach(f => CollectionBeforeChanges.Add((ClientsRequests)f.Clone()));
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список обращений\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddCommand { get; }

        private bool CanDeleteCommandExecute(object p) => true;
        private bool CanSaveCommandExecute(object p) => true;
        private bool CanAddCommandExecute(object p) => true;

        private void OnDeleteCommandExecuted(object p)
        {
            try
            {
                if (p is ClientsRequests model)
                {
                    if (model.Id != 0 && !new ConfirDeleteInCollection().run(0)) return;
                    if (model.Id != 0) db.Entry(model).State = EntityState.Deleted;
                    else db.Entry(model).State = EntityState.Detached;
                    int cnt = db.SaveChanges();
                    Collection = GetCollection();
                    if (cnt > 0) 
                    {
                        CollectionBeforeChanges.Clear();
                        Collection.ForEach(f => CollectionBeforeChanges.Add((ClientsRequests)f.Clone()));
                    }                 
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
                    if (string.IsNullOrEmpty(item.Fio) && item.ClientInfo == null)
                    {

                        var response = ThemedMessageBox.Show(title: "Внимание", text: "В списке имеются строки с одновременно незаполненными полями \"ФИО\" и \"Клиент\"! Данные строки не будут сохранены. Если хотите сохранить эти поля, то нажмите кнопку \"Отмена\", заполните одно из полей, а затем нажмите кнопку сохранить (иконка с дискетой). Для продолжения без сохранения, нажмите \"Ок\"",
                           messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "Cancel") return; else continue; 

                    }
                    if (item.Id == 0) db.Entry(item).State = EntityState.Added;        
                }
                int cnt = db.SaveChanges();
                Collection = GetCollection();
                CollectionBeforeChanges.Clear();
                Collection.ForEach(f => CollectionBeforeChanges.Add((ClientsRequests)f.Clone()));
                if (cnt > 0)
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

        private void OnAddCommandExecuted(object p) => Collection.Add(new ClientsRequests());

        public ObservableCollection<ClientsRequests> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<ClientsRequests> _Collection;
        private ObservableCollection<ClientsRequests> GetCollection() => db.ClientsRequests.OrderBy(d => d.CreatedAt).ToObservableCollection();
        public ObservableCollection<ClientsRequests> CollectionBeforeChanges { get; set; } = new ObservableCollection<ClientsRequests>();

        public bool HasUnsavedChanges()
        {
            if (CollectionBeforeChanges?.Count != Collection.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Fio) && item.ClientInfo == null) continue;
                if (item.Id == 0) return true;
                if (!item.Equals(CollectionBeforeChanges.Where(f => f.Guid == item.Guid).FirstOrDefault())) return true;
            }
            return false;
        }

        public bool UserSelectedBtnCancel()
        {

            var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Если хотите сохранить эти данные, то нажмите кнопку \"Отмена\", а затем кнопку сохранить (иконка с дискетой). Для продолжения без сохранения, нажмите \"Ок\"",
               messageBoxButtons: MessageBoxButton.OKCancel, icon: MessageBoxImage.Warning) ;

            return response.ToString() == "Cancel";
        }
    }
}
