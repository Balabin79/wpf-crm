using System.Data.Entity;


namespace Dental.Models
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection"){}

        public DbSet<Employee> Employes { get; set; }
        public DbSet<Advertising> Advertising { get; set; }
        public DbSet<DiscountGroups> DiscountGroups { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Organization> Organizations { get; set; }       
        public DbSet<TreatmentPlan> TreatmentPlanes { get; set; }
        public DbSet<EmployesSpecialities> EmployesSpecialities { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompany { get; set; }
        public DbSet<PatientInfo> PatientInfo { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Communication> Communication { get; set; }
        public DbSet<ClientsGroup> ClientsGroup { get; set; }
        public DbSet<Price> Price { get; set; }
        public DbSet<PriceRate> PriceRate { get; set; }
    }
}
