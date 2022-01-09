using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("SubscribesLog")]
    public class SubscribesLog : AbstractBaseModel, IDataErrorInfo
    {
        public int? ClientsSubscribesId { get; set; }
        public int? ClientInfoId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
