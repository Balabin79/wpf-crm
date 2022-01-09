using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("TypesSubscribe")]
    public class TypeSubscribe : AbstractBaseModel, IDataErrorInfo
    {
        public string Name { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override string ToString() => Name;
    }
}
