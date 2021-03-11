using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("EmployesSpecialities")]
    class EmployesSpecialities : AbstractBaseModel
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; }

        public bool this[PropertyInfo prop, EmployesSpecialities item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "EmployeeId": return item.EmployeeId == EmployeeId;
                    case "SpecialityId": return item.SpecialityId == SpecialityId;

                    default: return true;
                }
            }
        }

        public void Copy(EmployesSpecialities copy)
        {
            EmployeeId = copy.EmployeeId;
            SpecialityId = copy.SpecialityId;

        }
    }
}
