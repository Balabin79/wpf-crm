using System.Collections.Generic;

namespace Dental.Models
{
    public class Role
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }


        public List<Role> FakeListRoles
        {
            get
            {
                return new List<Role>
                {
                    new Role() {Name="Admin", Description="", IsActive=true},
                    new Role() {Name="Doctor", Description="", IsActive=true},
                    new Role() {Name="Employee", Description="", IsActive=true},
                    new Role() {Name="Guest", Description="", IsActive=true},
                    new Role() {Name="User", Description="", IsActive=true},
                    new Role() {Name="Patient", Description="", IsActive=true}
                };
            }

        }

    }
}
