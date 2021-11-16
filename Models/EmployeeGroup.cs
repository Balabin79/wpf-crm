using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("EmployeeGroups")]
    class EmployeeGroup : AbstractBaseModel, IDataErrorInfo
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

        [Display(Name = "Больше или меньше стоимости работы")]
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

        public object Clone()
        {
            Dictionary percentOrCost = new Dictionary
            {
                Name = this.PercentOrCost?.Name,
                Id = this.PercentOrCost?.Id ?? 0,
                CategoryId = this.PercentOrCost?.CategoryId ?? 0,
                Guid = this.PercentOrCost?.Guid
            };

            Dictionary moreOrLess = new Dictionary
            {
                Name = this.MoreOrLess?.Name,
                Id = this.MoreOrLess?.Id ?? 0,
                CategoryId = this.MoreOrLess?.CategoryId ?? 0,
                Guid = this.MoreOrLess?.Guid
            };

            return new EmployeeGroup
            {
                Id = this.Id,
                Name = this.Name,
                Guid = this.Guid,
                IsActive = this.IsActive,
                Amount = this.Amount,
                IsApplyRule = this.IsApplyRule,
                PercentOrCost = percentOrCost,
                PercentOrCostId = this.PercentOrCostId,
                MoreOrLess = moreOrLess,
                MoreOrLessId = this.MoreOrLessId
            };
        }

        public EmployeeGroup Copy(EmployeeGroup model)
        {
            model.Id = this.Id;
            model.Name = this.Name;
            model.Guid = this.Guid;
            model.IsActive = this.IsActive;
            model.Amount = this.Amount;
            model.IsApplyRule = this.IsApplyRule;
            model.MoreOrLess = this.MoreOrLess;
            model.PercentOrCost = this.PercentOrCost;
            return model;
        }


        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as EmployeeGroup);
        }
        public bool Equals(EmployeeGroup other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Name, other.Name);
            StringParamsIsEquel(this.Guid, other.Guid);
            StringParamsIsEquel(this.MoreOrLess?.Guid, other.MoreOrLess?.Guid);
            StringParamsIsEquel(this.PercentOrCost?.Guid, other.PercentOrCost?.Guid);
            StringParamsIsEquel(this.Amount, other.Amount);
            if (this.IsActive != other.IsActive) return false;
            if (this.IsApplyRule != other.IsApplyRule) return false;
            return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
        }

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;

    }
}
