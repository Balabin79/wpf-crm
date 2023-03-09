using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using Dental.Services;
//using System.Data.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Windows.Forms;

namespace Dental.Models
{
    public class ApplicationContext : DbContext
    {
        private readonly int typeDb;
        private readonly string conn;
        public ApplicationContext(string connection, int type = 0) 
        { typeDb = type; conn = connection;
            Database.EnsureCreated();
        }
        //public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options) { }
       // public ApplicationContext(string connectionString) : base(connectionString) { }
        //public ApplicationContext(SqliteConnection sQLiteConnection) : base(sQLiteConnection) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (typeDb == 0 ) optionsBuilder.UseSqlite(@"Data Source=" + conn);
            if (typeDb == 1 ) optionsBuilder.UseNpgsql(@"Host=localhost;Port=5433;Database=B6Crm2;Username=postgres;Password=657913;");
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
