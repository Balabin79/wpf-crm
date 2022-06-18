using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("InvoiceMaterialItems")]
    public class InvoiceMaterialItems : AbstractBaseModel, IDataErrorInfo
    {
       public Nomenclature Nomenclature { get; set; }
       public int? NomenclatureId { get; set; }
       
       public Invoice Invoice { get; set; }
       public int? InvoiceId { get; set; }

       public int Count { get; set; }
       public decimal Price { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is InvoiceMaterialItems model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if (
                    StringParamsIsEquel(Invoice?.Guid, model.Invoice?.Guid) &&
                    StringParamsIsEquel(Nomenclature?.Guid, model.Nomenclature?.Guid) &&
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
