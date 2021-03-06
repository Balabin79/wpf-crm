using Dental.Interfaces;
using Dental.Models.Template;
using System.Data.Entity;


namespace Dental.Models
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employes { get; set; }
        public DbSet<Advertising> Advertising { get; set; }
        public DbSet<DiscountGroups> DiscountGroups { get; set; }
        public DbSet<LoyaltyPrograms> LoyaltyPrograms { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Diagnos> Diagnoses { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlanes { get; set; }
        public DbSet<InitialInspection> InitialInspectiones { get; set; }
        public DbSet<EmployeeStatus> EmployeeStatuses { get; set; }
    }
}
