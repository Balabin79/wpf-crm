using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("PurchaseInvoice")]
    public class PurchaseInvoice : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Номенклатура"" обязательно для заполнения")]
        [Display(Name = "Номенклатура")]
        public Nomenclature Nomenclature { get; set; }
        public int? NomenclatureId { get; set; }

        [Required(ErrorMessage = @"Поле ""Дата поступления"" обязательно для заполнения")]
        public string ReceiptDate { get; set; }

        [Required(ErrorMessage = @"Поле ""Количество"" обязательно для заполнения")]
        public int Cnt { get; set; }

        public Warehouse Warehouse { get; set; }
        public int? WarehouseId { get; set; }

        public Contractor Contractor { get; set; }
        public int? ContractorId { get; set; }

        public decimal PurchasePrice { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
        
        public override bool Equals(object other)
        {
            if (other is PurchaseInvoice clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.Nomenclature?.Guid, clone.Nomenclature?.Guid)
                    && StringParamsIsEquel(this.Guid, clone.Guid)
                    && StringParamsIsEquel(this.ReceiptDate, clone.ReceiptDate)
                    && StringParamsIsEquel(this.Warehouse?.Guid, clone.Warehouse?.Guid)
                    && StringParamsIsEquel(this.Contractor?.Guid, clone.Contractor?.Guid)
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
