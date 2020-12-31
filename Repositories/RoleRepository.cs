using Dental.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;

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

        public static ObservableCollection<Role> GetRoles()
        {
            ApplicationContext db = new ApplicationContext();
            db.Roles.Load();
            return db.Roles.Local;
        }
    }
}
