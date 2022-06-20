using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("InvoiceServiceItems")]
    public class InvoiceServiceItems : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {
        public Service Service 
        { 
            get => service; 
            set
            {
                service = value;
                OnPropertyChanged(nameof(Service));
            } 
        }
        public int? ServiceId { get; set; }
        private Service service;
       
       public Invoice Invoice { get; set; }
       public int? InvoiceId { get; set; }       
        
        public Employee Employee 
        {
            get => employee; 
            set
            {
                employee = value;
                OnPropertyChanged(nameof(Employee));
            } 
        }
        public int? EmployeeId { get; set; }
        private Employee employee;

        public int Count 
        { 
            get => count;
            set
            {
                count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
        private int count;

        public decimal Price 
        { 
            get => price;
            set
            {
                price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        private decimal price;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is InvoiceServiceItems model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if (
                    StringParamsIsEquel(Invoice?.Guid, model.Invoice?.Guid) &&
                    StringParamsIsEquel(Service?.Guid, model.Service?.Guid) &&
                    Count == model.Count && Price == model.Price
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
