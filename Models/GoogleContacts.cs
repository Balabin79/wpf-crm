using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models
{
    public class GoogleContacts : AbstractBaseModel
    {
        public string Email { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public string CalendarName { get; set; }

        public override string ToString() => CalendarName;
    }
}
