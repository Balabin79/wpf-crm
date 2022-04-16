using System;
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
using Dental.Services;
using Dental.Views.Documents;

namespace Dental.ViewModels
{
    class DocumentsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public DocumentsViewModel()
        {
            OpenFormPurchaseInvoiceCommand = new LambdaCommand(OnOpenFormPurchaseInvoiceExecuted, CanOpenFormPurchaseInvoiceExecute);
            OpenFormMovementGoodsCommand = new LambdaCommand(OnOpenFormMovementGoodsExecuted, CanMovementGoodsExecute);
            OpenFormCompletionActCommand = new LambdaCommand(OnOpenFormCompletionActExecuted, CanCompletionActExecute);

            try
            {
                db = new ApplicationContext();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Документы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand OpenFormPurchaseInvoiceCommand { get; }
        public ICommand OpenFormMovementGoodsCommand { get; }
        public ICommand OpenFormCompletionActCommand { get; }

        private bool CanOpenFormPurchaseInvoiceExecute(object p) => true;
        private bool CanMovementGoodsExecute(object p) => true;
        private bool CanCompletionActExecute(object p) => true;

        private void OnOpenFormPurchaseInvoiceExecuted(object p)
        {
            try
            {
                PurchaseInvoiceWindow = new PurchaseInvoiceWindow();
                PurchaseInvoiceWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormMovementGoodsExecuted(object p)
        {
            try
            {
                MovementGoodsWindow = new MovementGoodsWindow();
                MovementGoodsWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormCompletionActExecuted(object p)
        {
            try
            {
                CompletionActWindow = new CompletionActWindow();
                CompletionActWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }


        public CompletionActWindow CompletionActWindow { get; set;  }
        public MovementGoodsWindow MovementGoodsWindow { get; set;  }
        public PurchaseInvoiceWindow PurchaseInvoiceWindow { get; set;  }


        /***/
        private void OnAddCommandExecuted(object p) => Collection.Add(new Advertising());

        public ObservableCollection<Advertising> Collection
        {
            get => _Collection;
            set => Set(ref _Collection, value);
        }
        private ObservableCollection<Advertising> _Collection;
        private ObservableCollection<Advertising> GetCollection() => db.Advertising?.OrderBy(d => d.Name).ToObservableCollection();
        public ObservableCollection<Advertising> CollectionBeforeChanges { get; set; } = new ObservableCollection<Advertising>();

        public bool HasUnsavedChanges()
        {
            /*if (CollectionBeforeChanges?.Count != Collection.Count) return true;
            foreach (var item in Collection)
            {
                if (string.IsNullOrEmpty(item.Name)) continue;
                if (item.Id == 0) return true;
                if (!item.Equals(CollectionBeforeChanges.Where(f => f.Guid == item.Guid).FirstOrDefault())) return true;
            }*/
            return false;
        }

        public bool UserSelectedBtnCancel()
        {
            /*var response = ThemedMessageBox.Show(title: "Внимание", text: "Имеются несохраненные изменения! Закрыть без сохранения?",
               messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning) ;

            return response.ToString() == "No";*/
            return true;
        }
    }
}
