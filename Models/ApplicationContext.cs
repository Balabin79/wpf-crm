using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;

namespace Dental.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection"){}

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is AbstractBaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entity in entities) ((AbstractBaseModel)entity.Entity).Update();           
        }

        public DbSet<Employee> Employes { get; set; }
        public DbSet<Advertising> Advertising { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<Organization> Organizations { get; set; }       
        public DbSet<EmployesSpecialities> EmployesSpecialities { get; set; }
        public DbSet<PatientInfo> PatientInfo { get; set; }
        public DbSet<Communication> Communication { get; set; }
        public DbSet<ClientsGroup> ClientsGroup { get; set; }
        public DbSet<EmployeeGroup> EmployeeGroup { get; set; }
        public DbSet<Classificator> Classificator { get; set; }
        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<Teeth> Teeth { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlan { get; set; }
        public DbSet<TreatmentPlanItems> TreatmentPlanItems { get; set; }
        public DbSet<InvoiceItems> InvoiceItems { get; set; }

        public DbSet<MedicalAppointment> Appointments { get; set; }
        public DbSet<ResourceEntity> Resources { get; set; }
    }
}
