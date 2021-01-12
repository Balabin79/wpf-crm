using Dental.Models.Template;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<DiagnosRepository> Diagnoses { get; set; }
        public DbSet<Diary> Diaries { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlanes { get; set; }
        public DbSet<InitialInspection> InitialInspectiones { get; set; }
    }
}
