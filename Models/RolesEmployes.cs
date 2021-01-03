using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Dental.Models
{
    class RolesEmployes
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int OrganizationId { get; set; }
    }
}
