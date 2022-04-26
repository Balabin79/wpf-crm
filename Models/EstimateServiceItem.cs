using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("EstimateServiceItems")]
    public class EstimateServiceItem : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Услуга"" обязательно для заполнения")]
        public Service Service { get; set; }
        public int? ServiceId { get; set; }

        public Estimate Estimate { get; set; }
        public int? EstimateId { get; set; }

        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }

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
                    StringParamsIsEquel(this.Service?.Guid, clone.Service?.Guid) &&
                    StringParamsIsEquel(this.Estimate?.Guid, clone.Estimate?.Guid) &&
                    StringParamsIsEquel(this.Employee?.Guid, clone.Employee?.Guid) &&
                    this.Price == clone?.Price &&
                    this?.Count == clone?.Count
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

        public void Update()
        {
            OnPropertyChanged(nameof(Service));
            OnPropertyChanged(nameof(Employee));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Price));
        }
    }
}
