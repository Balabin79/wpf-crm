using B6CRM.Models.Base;
using DevExpress.Mvvm;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
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

        public int? AdvertisingId
        {
            get { return GetProperty(() => AdvertisingId); }
            set { SetProperty(() => AdvertisingId, value); }
        }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Advertising Advertising
        {
            get { return GetProperty(() => Advertising); }
            set { SetProperty(() => Advertising, value); }
        }

        public string Number { get; set; }
        public int? Paid { get; set; } = 0;

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }
        public int? ClientId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
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

        [DeleteBehavior(DeleteBehavior.SetNull)]
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

        public object Clone() => MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is Invoice model)
            {
                if (ReferenceEquals(this, model)) return true;
                if (
                    StringParamsIsEquel(Number, model.Number) &&
                    StringParamsIsEquel(Date, model.Date) &&
                    StringParamsIsEquel(Client?.Guid, model.Client?.Guid) &&
                    Paid == model.Paid
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
