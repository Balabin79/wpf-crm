using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Dental.ViewModels.Invoices
{
    public class InvoiceVM : BindableBase, IDataErrorInfo
    {
        public string Number
        {
            get { return GetProperty(() => Number); }
            set { SetProperty(() => Number, value); }
        }

        [Required(ErrorMessage = @"Поле ""Клиент"" обязательно для заполнения")]
        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }

        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }

        public int? Paid
        {
            get { return GetProperty(() => Paid); }
            set { SetProperty(() => Paid, value); }
        }

        public decimal Total
        {
            get { return GetProperty(() => Total); }
            set { SetProperty(() => Total, value); }
        }

        public Visibility ClientFieldVisibility
        {
            get { return GetProperty(() => ClientFieldVisibility); }
            set { SetProperty(() => ClientFieldVisibility, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public ICollection<Client> Clients { get; set; }
    }
}
