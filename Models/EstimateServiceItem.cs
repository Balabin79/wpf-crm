using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("EstimateServiceItems")]
    public class EstimateServiceItem : AbstractBaseModel, IDataErrorInfo
    {
        public Estimate Estimate 
        { 
            get => estimate; 
            set
            {
                estimate = value;
                OnPropertyChanged(nameof(Estimate));
            } 
        }
        public int EstimateId { get; set; }
        private Estimate estimate;

        [Required(ErrorMessage = @"Поле ""Услуга"" обязательно для заполнения")]
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

        public override bool Equals(object other)
        {
            if (other is EstimateServiceItem clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.Estimate?.Guid, clone.Estimate?.Guid) &&
                    StringParamsIsEquel(this.Service?.Guid, clone.Service?.Guid) &&
                    StringParamsIsEquel(this.Employee?.Guid, clone.Employee?.Guid) &&
                    this?.Price == clone?.Price &&
                    this?.Count == clone?.Count 
                ) return true;
            }
            return false;
        }

        public override int GetHashCode() => Guid.GetHashCode();

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }
    }

}
