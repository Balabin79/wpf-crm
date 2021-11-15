using System.Data.Entity;


namespace Dental.Models
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection"){}

        public DbSet<Employee> Employes { get; set; }
        public DbSet<Advertising> Advertising { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Organization> Organizations { get; set; }       
        public DbSet<EmployesSpecialities> EmployesSpecialities { get; set; }
        public DbSet<PatientInfo> PatientInfo { get; set; }
        public DbSet<Communication> Communication { get; set; }
        public DbSet<ClientsGroup> ClientsGroup { get; set; }
        public DbSet<Classificator> Classificator { get; set; }
        public DbSet<ClientTreatmentPlans> ClientTreatmentPlans { get; set; }
        public DbSet<ClientFiles> ClientFiles { get; set; }
    }
}
