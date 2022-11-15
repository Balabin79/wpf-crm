using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("IndividualPrice")]
    public class IndividualPrice : AbstractBaseModel
    {
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public decimal? Price { get; set; }
    }
}
