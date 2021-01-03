using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Dental.Models
{
    class EmployesSpecialities
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int SpecialityId { get; set; }
    }
}
