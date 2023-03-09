using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Windows.Forms;
using Dental.Services;

namespace Dental.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() => Config = new Config();
     
        public Config Config { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Config.DbType == 0 ) optionsBuilder.UseSqlite(@"Data Source=" + Config.ConnectionString);
            if (Config.DbType == 1 ) optionsBuilder.UseNpgsql(Config.ConnectionString);
        }

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
            foreach (var entity in entities)
            {
                ((AbstractBaseModel)entity.Entity).Update();
            }
        }

        public DbSet<Employee> Employes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<LocationAppointment> LocationAppointment { get; set; }
        public DbSet<ShedulerStatuses> ShedulerStatuses { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatus { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItems> InvoiceItems { get; set; }

        public DbSet<AdditionalClientField> AdditionalClientFields { get; set; }
        public DbSet<TemplateType> TemplateType { get; set; }
        public DbSet<CommonValue> CommonValues { get; set; }
        public DbSet<AdditionalClientValue> AdditionalClientValue { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public DbSet<RoleManagment> RolesManagment { get; set; }

        public DbSet<ClientCategory> ClientCategories { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Advertising> Advertising { get; set; }
    }
}
