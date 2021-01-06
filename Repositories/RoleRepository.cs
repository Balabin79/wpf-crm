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
                    new Role() {Name="Admin", Description=""},
                    new Role() {Name="Doctor", Description=""},
                    new Role() {Name="Employee", Description=""},
                    new Role() {Name="Guest", Description=""},
                    new Role() {Name="User", Description=""},
                    new Role() {Name="Patient", Description=""}
                };
        }

        public static ObservableCollection<Role> GetRoles()
        {
            ApplicationContext db = new ApplicationContext();
            db.Roles.Load();
            return db.Roles.Local;
        }

        public static void Add(Role role)
        {
            ApplicationContext db = new ApplicationContext();
            db.Roles.Add(role);
            db.SaveChanges();
        }

        public static void Update(Role role)
        {
            ApplicationContext db = new ApplicationContext();

            var result = db.Roles.Find(role.Id);
            if (result != null)
            {
                result.Name = role.Name;
                result.Description = role.Description;

                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Role role)
        {
            ApplicationContext db = new ApplicationContext();
            db.Entry(role).State = EntityState.Deleted;
            db.Roles.Remove(role);
           int cnt = db.SaveChanges();
        }
    }
}
