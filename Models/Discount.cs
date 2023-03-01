using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;

namespace Dental.Models
{
    //[Table("AdditionalClientFields")]
    public class Discount : AbstractBaseModel, IDataErrorInfo
    {

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
    }
}
