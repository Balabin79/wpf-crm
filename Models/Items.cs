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
    [Table("ServicePlansItems")]
    public class Items : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {


        [Required(ErrorMessage = @"Поле ""Классификатор"" обязательно для заполнения")]
        public Service Classificator { get; set; }
        public int? ClassificatorId { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int Count { get; set; }
        public string Price { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is Items clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.Price, clone.Price) &&
                    this?.Classificator == clone?.Classificator &&
                    this?.Employee == clone?.Employee &&
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
            OnPropertyChanged(nameof(Classificator));
            OnPropertyChanged(nameof(Employee));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Price));
        }
    }

}
