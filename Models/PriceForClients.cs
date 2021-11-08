using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("PriceForClients")]
    class PriceForClients : AbstractBaseModel, IDataErrorInfo
    {
        public int? ClassificatorId { get; set; }
        public Classificator Classificator { get; set; }

        public int? PriceRateForClientsId { get; set; }
        public PriceRateForClients PriceRateForClients { get; set; }

        public string Price { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
