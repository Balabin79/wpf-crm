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
                PurchaseInvoices = db.PurchaseInvoice.Include(f => f).Include(f => f.Nomenclature).Include(f => f.Contractor).OrderBy(f => f.PurchasePrice).ToObservableCollection();

            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Поступление товаров\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICommand OpenClientCardCommand { get; }
       

        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }

    }
}