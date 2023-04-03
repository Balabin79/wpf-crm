using B6CRM.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;

namespace B6CRM.Models
{
    //[Table("AdditionalClientFields")]
    public class Discount : AbstractBaseModel, IDataErrorInfo
    {

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => MemberwiseClone();
    }
}
