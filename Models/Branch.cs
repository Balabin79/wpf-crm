using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("Branches")]
    public class Branch : AbstractBaseModel
    {
        public string WorkTime { get; set; }
        public int? BranchId { get; set; }
    }
}
