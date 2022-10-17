﻿using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dental.Models.Templates;

namespace Dental.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {

        }

        /*
        public List<ValidationResult> CustomErrors { get; set; } = new List<ValidationResult>();

        public void CustomValidate()
        {
            var changedEntities = ChangeTracker.Entries().Where(_ => _.State == EntityState.Added || _.State == EntityState.Modified);

            foreach (var e in changedEntities)
            {
                var vc = new ValidationContext(e.Entity, null, null);
                Validator.TryValidateObject(e.Entity, vc, CustomErrors, validateAllProperties: true);
            }

        }
        */

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

        public DbSet<UserActions> UserActions { get; set; }       
        public DbSet<Nomenclature> Nomenclature { get; set; }     
        public DbSet<Measure> Measure { get; set; }  

        public DbSet<Invoice> Invoices { get; set; }   
        public DbSet<InvoiceMaterialItems> InvoiceMaterialItems { get; set; }   
        public DbSet<InvoiceServiceItems> InvoiceServiceItems { get; set; }   

        public DbSet<AdditionalClientField> AdditionalClientFields { get; set; }   
        public DbSet<AdditionalEmployeeField> AdditionalEmployeeFields { get; set; }   
        public DbSet<TemplateType> TemplateType { get; set; }   
        public DbSet<CommonValue> CommonValues { get; set; }   
        public DbSet<AdditionalEmployeeValue> AdditionalEmployeeValue { get; set; }   
        public DbSet<AdditionalClientValue> AdditionalClientValue { get; set; }         
        
        public DbSet<IndividualPrice> IndividualPrice { get; set; }   

        public DbSet<Diagnos> Diagnoses { get; set; }   
        public DbSet<Diary> Diaries { get; set; }   
        public DbSet<Allergy> Allergies { get; set; }   
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }          
        public DbSet<Anamnes> Anamneses { get; set; }   
        public DbSet<Complaint> Complaints { get; set; }   
        public DbSet<DescriptionXRay> DescriptionXRay { get; set; }   
        public DbSet<Objectively> Objectively { get; set; }   

        public DbSet<TreatmentStage> TreatmentStage { get; set; }         
        public DbSet<Setting> Settings { get; set; }   

        public DbSet<RoleManagment> RolesManagment { get; set; }
        public DbSet<Reestr> Reestr { get; set; }

        public DbSet<Organization> Organizations { get; set; }
    }
}
