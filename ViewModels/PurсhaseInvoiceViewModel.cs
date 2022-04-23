using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
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
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;

namespace Dental.ViewModels
{
    public class PurсhaseInvoiceViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public PurсhaseInvoiceViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.PurchaseInvoice.Include(f => f.Nomenclature).Include(f => f.Counterparty).OrderBy(f => f.PurchasePrice).ToObservableCollection();                

                OpenFormPurchaseInvoice = new LambdaCommand(OnOpenFormPurchaseExecuted, CanOpenFormPurchaseExecute);
                SelectPosInClassificatorCommand = new LambdaCommand(OnSelectPosInClassificatorCommandExecuted, CanSelectPosInClassificatorCommandExecute);
                SaveCommand = new LambdaCommand(OnSaveExecuted, CanSaveExecute);
                DeleteCommand = new LambdaCommand(OnDeleteExecuted, CanDeleteExecute);
                CancelFormCommand = new LambdaCommand(OnCancelFormExecuted, CanCancelFormExecute);                
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Поступление товаров\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand OpenFormPurchaseInvoice { get; set; }
        public ICommand SelectPosInClassificatorCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelFormCommand { get; }

        private bool CanSaveExecute(object p) => true;
        private bool CanDeleteExecute(object p) => true;
        private bool CanCancelFormExecute(object p) => true;
        private bool CanOpenFormPurchaseExecute(object p) => true;
        private bool CanSelectPosInClassificatorCommandExecute(object p) => true;

        private void OnCancelFormExecuted(object p) => PurchaseInvoiceWindow?.Close();

        private void OnSaveExecuted(object p)
        {
            try
            {
                if (Model?.Id == 0) Add();
                else Update();

                
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnDeleteExecuted(object p)
        {
            try
            {

            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnOpenFormPurchaseExecuted(object p)
        {
            try
            {

                if (int.TryParse(p.ToString(), out int val) && val > 0) SetStateModel(false, val);
                else SetStateModel();

                NomenclatureList = db.Nomenclature.ToList();
                //Counterparties = db.Counterparties.ToList();

                PurchaseInvoiceWindow = new PurchaseInvoiceWindow();
                PurchaseInvoiceWindow.DataContext = this;
                PurchaseInvoiceWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        private void OnSelectPosInClassificatorCommandExecuted(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Nomenclature classificator)
                    {
                        if (classificator.IsDir == 1) return;
                        parameters.Popup.EditValue = classificator;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public PurchaseInvoiceWindow PurchaseInvoiceWindow { get; set; }

        public string Title 
        {
            get => title;
            set => Set(ref title, value);  
        }
        private string title;

        public PurchaseInvoice Model 
        { 
            get => model;
            set => Set(ref model, value);
        }
        private PurchaseInvoice model;

        public ICollection<PurchaseInvoice> Collection { get; set; }
        public ICollection<Nomenclature> NomenclatureList { get; set; }
        public ICollection<Counterparty> Counterparties { get; set; }

        private void SetStateModel(bool isNew = true, int id = 0)
        {
            if (isNew)
            {
                Model = new PurchaseInvoice();
                Model.Date = DateTime.Now.ToShortDateString();
                Title = "Новый документ";
            }
            else
            {
                Model = db.PurchaseInvoice.Where(f => f.Id == id).FirstOrDefault();
                Title = "Редактирование документа № " + Model.Id;
            }
        }


        private void Add()
        {
            db.Entry(Model).State = EntityState.Added;
            int rows = db.SaveChanges();

            if (rows > 0)
            {
                Collection.Add(Model);
                ActionsLog.RegisterAction(Model.Nomenclature.Name, ActionsLog.ActionsRu["add"], ActionsLog.SectionPage["PurchaseInvoice"]);
                var notification = new Notification();
                notification.Content = "Новый документ успешно записан в базу данных!";
                notification.run();
                Title = "Документ №" + Model?.Id;
            }
        }
        private void Update()
        {
            db.Entry(Model).State = EntityState.Modified;
            int rows = db.SaveChanges();
 
            if (rows > 0)
            {
                var notification = new Notification();
                notification.Content = "Отредактированный документ успешно сохранен в базу данных!";
                notification.run();
                //Model.UpdateFields();
                ActionsLog.RegisterAction(Model.Nomenclature.Name, ActionsLog.ActionsRu["edit"], ActionsLog.SectionPage["PurchaseInvoice"]);
            }
        }

        private void Delete(ObservableCollection<PurchaseInvoice> collection)
        {
            ActionsLog.RegisterAction(Model.Nomenclature.Name, ActionsLog.ActionsRu["delete"], ActionsLog.SectionPage["PurchaseInvoice"]);

            collection.ForEach(f => db.Entry(f).State = EntityState.Deleted);
            collection.ForEach(f => Collection.Remove(f));
        }

    }
}