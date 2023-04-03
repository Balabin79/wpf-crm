using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("RolesManagment")]
    public class RoleManagment : AbstractBaseModel, ICategoryTree
    {
        public string PageName { get; set; }
        public string PageTitle { get; set; }

        public int? DoctorAccess { get; set; } = 0;

        public int? AdminAccess { get; set; } = 0;

        public int? ReceptionAccess { get; set; } = 0;

        public int? ParentId { get; set; }
        public int? IsCategory { get; set; }

        public int? Num { get; set; }
    }
}
