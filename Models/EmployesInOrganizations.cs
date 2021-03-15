using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using System.Reflection;

namespace Dental.Models
{
    [Table("EmployesInOrganizations")]
    class EmployesInOrganizations : AbstractBaseModel
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public bool this[PropertyInfo prop, EmployesInOrganizations item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "EmployeeId": return item.EmployeeId == EmployeeId;
                    case "OrganizationId": return item.OrganizationId == OrganizationId;

                    default: return true;
                }
            }
        }

        public void Copy(EmployesInOrganizations copy)
        {
            EmployeeId = copy.EmployeeId;
            OrganizationId = copy.OrganizationId;

        }
    }
}
