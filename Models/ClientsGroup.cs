using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsGroup")]
    public class ClientsGroup : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set => _Name = value?.Trim();
        }
        private string _Name;

        [Display(Name = "Активно")]
        public int? IsActive { get; set; } = 1;

        [Display(Name = "Применяется правило")]
        public int? IsApplyRule { get; set; } = 0;

        [Display(Name = "Больше или меньше тарифа")]
        public Dictionary MoreOrLess { get; set; }
        public int? MoreOrLessId { get; set; }

        [Display(Name = "Процент или сумма")]
        public Dictionary PercentOrCost { get; set; }
        public int? PercentOrCostId { get; set; }

        [Display(Name = "Значение")]
        public string Amount
        {
            get => _Amount;
            set => _Amount = value?.Trim();
        }
        private string _Amount;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
          
        public override bool Equals(object other)
        {
            if (other is ClientsGroup clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Name, clone.Name) && 
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.Amount, clone.Amount) &&
                    this?.IsActive == clone?.IsActive &&
                    this?.IsApplyRule == clone?.IsApplyRule &&
                    this?.MoreOrLess == clone?.MoreOrLess &&
                    this?.PercentOrCost == clone?.PercentOrCost 
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

        public override string ToString() => Name;      
    }
}
