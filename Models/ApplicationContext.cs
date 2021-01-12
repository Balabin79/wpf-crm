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
        public DbSet<ITreeViewCollection> Diagnoses { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlanes { get; set; }
        public DbSet<InitialInspection> InitialInspectiones { get; set; }
    }
}
