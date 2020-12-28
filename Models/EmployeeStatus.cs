using System.Collections.Generic;

namespace Dental.Models
{
    public class EmployeeStatus
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }



        public List<EmployeeStatus> FakeListEmployeeStatus        {
            get
            {
                return new List<EmployeeStatus>
            {
                new EmployeeStatus() {Name="Работает", Description="", IsActive=true},
                new EmployeeStatus() {Name="Уволен", Description="", IsActive=true},
                new EmployeeStatus() {Name="В отпуске", Description="", IsActive=true},

            };
            }

        }
    }
}
