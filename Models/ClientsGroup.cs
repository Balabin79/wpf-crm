using Dental.Infrastructures.Logs;
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

        public object Clone()
        {
            try
            {
                return new ClientsGroup
                {
                    Id = this.Id,
                    Name = this.Name,
                    Guid = this.Guid,
                    IsActive = this.IsActive,
                    Amount = this.Amount,
                    IsApplyRule = this.IsApplyRule,
                    PercentOrCost = this.PercentOrCost,
                    PercentOrCostId = this.PercentOrCostId,
                    MoreOrLess = this.MoreOrLess,
                    MoreOrLessId = this.MoreOrLessId
                };
            } catch(Exception ex)
            {
                (new ViewModelLog(ex)).run();
                return new ClientsGroup();
            }

        }

        public ClientsGroup Copy(ClientsGroup model)
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

            return this.Equals(other as ClientsGroup);
        }
        public bool Equals(ClientsGroup other)
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
            StringParamsIsEquel(this.Amount, other.Amount);
            StringParamsIsEquel(this.PercentOrCost?.Guid, other.PercentOrCost?.Guid);
            StringParamsIsEquel(this.MoreOrLess?.Guid, other.MoreOrLess?.Guid);
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

        public override string ToString() => Name;      
    }
}
