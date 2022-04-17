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
                PurchaseInvoices = db.PurchaseInvoice.Include(f => f.Nomenclature).Include(f => f.Warehouse).Include(f => f.Contractor).OrderBy(f => f.PurchasePrice).ToObservableCollection();                

                OpenFormPurchaseInvoice = new LambdaCommand(OnOpenFormPurchaseExecuted, CanOpenFormPurchaseExecute);

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Поступление товаров\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand OpenFormPurchaseInvoice { get; set; }

        private bool CanOpenFormPurchaseExecute(object p) => true;

        private void OnOpenFormPurchaseExecuted(object p)
        {
            try
            {
                NomenclatureList = db.Nomenclature.ToList();
                PurchaseInvoiceWindow = new PurchaseInvoiceWindow();
                PurchaseInvoiceWindow.DataContext = this;
                PurchaseInvoiceWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public PurchaseInvoiceWindow PurchaseInvoiceWindow { get; set; }

        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        public ICollection<Nomenclature> NomenclatureList { get; set; }

    }
}