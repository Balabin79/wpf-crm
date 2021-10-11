using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsByAdvertising")]
    class ClientsByAdvertising : AbstractBaseModel, IDataErrorInfo
    {
        public int? AdvertisingId { get; set; }
        public Advertising Advertising { get; set; }

        public int? PatientInfoId { get; set; }
        public PatientInfo PatientInfo { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
