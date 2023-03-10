using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Invoices")]
    public class Invoice : AbstractBaseModel, IDataErrorInfo
    {
        public Invoice()
        {
            InvoiceItems = new ObservableCollection<InvoiceItems>();
        }

        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }

        public long? DateTimestamp
        {
            get { return GetProperty(() => DateTimestamp); }
            set { SetProperty(() => DateTimestamp, value); }
        }

        public int? AdvertisingId { get; set; }
        public Advertising Advertising { get; set; }

        public string Number { get; set; }
        public int? Paid { get; set; } = 0;

        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }
        public int? ClientId { get; set; }

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }
        public int? EmployeeId { get; set; }

        public ObservableCollection<InvoiceItems> InvoiceItems
        {
            get { return GetProperty(() => InvoiceItems); }
            set { SetProperty(() => InvoiceItems, value); }
        }

        public Discount Discount
        {
            get { return GetProperty(() => Discount); }
            set { SetProperty(() => Discount, value); }
        }

        public int? DiscountId { get; set; }

        public decimal? DiscountSum
        {
            get { return GetProperty(() => DiscountSum); }
            set { SetProperty(() => DiscountSum, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is Invoice model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if (
                    StringParamsIsEquel(Number, model.Number) && 
                    StringParamsIsEquel(Date, model.Date) &&
                    StringParamsIsEquel(Client?.Guid, model.Client?.Guid) && 
                    this.Paid == model.Paid
                    ) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }
    }
}
