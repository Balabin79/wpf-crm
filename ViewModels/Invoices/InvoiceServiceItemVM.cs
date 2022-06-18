﻿using Dental.Models;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.ViewModels.Invoices
{
    public class InvoiceServiceItemVM : BindableBase, IDataErrorInfo
    {
        public Invoice Invoice
        {
            get { return GetProperty(() => Invoice); }
            set { SetProperty(() => Invoice, value); }
        }

        [Required(ErrorMessage = @"Поле ""Услуга"" обязательно для заполнения")]
        public Service Service
        {
            get { return GetProperty(() => Service); }
            set { SetProperty(() => Service, value); }
        }

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }

        public int Count
        {
            get { return GetProperty(() => Count); }
            set { SetProperty(() => Count, value); }
        }

        public decimal Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
