using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Views.PatientCard;
using Dental.Views.WindowForms;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Services;
using Dental.Models;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Documents;

namespace Dental.ViewModels
{
    public class PatientListViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public PatientListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                SetCollection();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Список клиентов\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenFormDocuments()
        {
            try
            {
                ClientsDocumentsWindow = new ClientsDocumentsWindow();
                ClientsDocumentsWindow.ShowDialog();
            }
            catch 
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenFormAdvertising()
        {
            try
            {
                AdvertisingWin = new AdvertisingWindow();
                AdvertisingWin.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OpenClientCard(object p)
        {
            try
            {
                ClientCardWin = (p != null) ? new ClientCardWindow((int)p, this) : new ClientCardWindow(0, this);
                ClientCardWin.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Карта клиента\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ShowArchive()
        {
            try
            {
                IsArchiveList = !IsArchiveList; 
                SetCollection(IsArchiveList);
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public bool IsArchiveList
        {
            get { return GetProperty(() => IsArchiveList); }
            set { SetProperty(() => IsArchiveList, value); }
        }

        public ObservableCollection<Client> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public AdvertisingWindow AdvertisingWin { get; set; } 
        public ClientsDocumentsWindow ClientsDocumentsWindow { get; set; }
        public ClientCardWindow ClientCardWin { get; set; }

        public void SetCollection(bool isArhive=false) => Collection = db.Clients.OrderBy(f => f.LastName).Where(f => f.IsInArchive == isArhive).ToObservableCollection();
    }
}