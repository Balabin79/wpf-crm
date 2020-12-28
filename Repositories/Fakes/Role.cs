using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dental.Repositories.Fakes
{
    public static class Role
    {

        public static List<Models.Role> ListRoles
        {
            get
            {
                return new List<Models.Role>
            {
                new Models.Role() {Name="Admin", Description="", IsActive=true},
                new Models.Role() {Name="Doctor", Description="", IsActive=true},
                new Models.Role() {Name="Employee", Description="", IsActive=true},
                new Models.Role() {Name="Guest", Description="", IsActive=true},
                new Models.Role() {Name="User", Description="", IsActive=true},
                new Models.Role() {Name="Patient", Description="", IsActive=true}
            };
            }

        }

    }
}
