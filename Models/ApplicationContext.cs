using Dental.Interfaces;
using Dental.Models.Template;
using Dental.Models.PatientCard;
using Dental.Models.Share;
using System.Data.Entity;


namespace Dental.Models
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection"){}

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
        public DbSet<EmployesSpecialities> EmployesSpecialities { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompany { get; set; }
        public DbSet<PatientInfo> PatientInfo { get; set; }
        public DbSet<Questionary> Questionary { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Nomenclature> Nomenclature { get; set; }
        public DbSet<Unit> Unit { get; set; }

        public DbSet<Storage> Storage { get; set; }
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<Branche> Branches { get; set; }
        public DbSet<Departament> Departaments { get; set; }
        public DbSet<CommunicationLog> CommunicationLog { get; set; }
        public DbSet<CommunicationLogStatus> CommunicationLogStatus { get; set; }
        public DbSet<CommunicationLogThema> CommunicationLogThema { get; set; }
        public DbSet<ClientsCategory> ClientsCategory { get; set; }
    }
}
