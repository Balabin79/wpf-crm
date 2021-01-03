using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    class EmployesInOrganizations
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int OrganizationId { get; set; }

    }
}
