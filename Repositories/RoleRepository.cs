using Dental.Models;
using System.Collections.ObjectModel;


namespace Dental.Repositories
{
    static class RoleRepository
    {

        public static ObservableCollection<Role> GetFakeRoles()
        {
            return new ObservableCollection<Role>
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
