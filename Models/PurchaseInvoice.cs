using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("PurchaseInvoice")]
    public class PurchaseInvoice : AbstractBaseModel, IDataErrorInfo, IDoc
    {
        [Required(ErrorMessage = @"Поле ""Номенклатура"" обязательно для заполнения")]
        [Display(Name = "Номенклатура")]
        public Nomenclature Nomenclature { get; set; }
        public int? NomenclatureId { get; set; }

        public int Cnt { get; set; }

        public Counterparty Counterparty { get; set; }
        public int? CounterpartyId { get; set; }

        public decimal PurchasePrice { get; set; }

        public string Date { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is PurchaseInvoice clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.Nomenclature?.Guid, clone.Nomenclature?.Guid)
                    && StringParamsIsEquel(this.Guid, clone.Guid)
                    && StringParamsIsEquel(this.Date, clone.Date)
                    && StringParamsIsEquel(this.Counterparty?.Guid, clone.Counterparty?.Guid)
                    && this.PurchasePrice != clone.PurchasePrice
                    && this.Cnt != clone.Cnt
                    ) 
                    return true;
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
