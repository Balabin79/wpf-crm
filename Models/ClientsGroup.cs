using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsGroup")]
    class ClientsGroup : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Вид скидки")]
        public string DiscountType { get; set; }

        [Display(Name = "Размер скидки")]
        public string Amount 
        {
            get => string.Format("{0:C}", _Amount);
            set => _Amount = value;
        }
        private string _Amount;

        public int? IsDiscountActive { get; set; } = 0;

        public int? IsApplyDiscount { get; set; } = 0;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new ClientsGroup
            {
                Id = this.Id,
                Name = this.Name,
                Guid = this.Guid,
                DiscountType = this.DiscountType,
                Amount = this.Amount,
                IsDiscountActive = this.IsDiscountActive,
                IsApplyDiscount = this.IsApplyDiscount
        };
        }

        public ClientsGroup Copy(ClientsGroup model)
        {
            model.Id = this.Id;
            model.Name = this.Name;
            model.Guid = this.Guid;
            model.DiscountType = this.DiscountType;
            model.Amount = this.Amount;
            model.IsDiscountActive = this.IsDiscountActive;
            model.IsApplyDiscount = this.IsApplyDiscount;
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
            StringParamsIsEquel(this.DiscountType, other.DiscountType);
            StringParamsIsEquel(this.Amount, other.Amount);
            if (this.IsDiscountActive != other.IsDiscountActive) return false;
            if (this.IsApplyDiscount != other.IsApplyDiscount) return false;
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
