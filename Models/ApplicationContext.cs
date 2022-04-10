﻿using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
            foreach (var entity in entities) ((AbstractBaseModel)entity.Entity).Update();           
        }

        public DbSet<Employee> Employes { get; set; }
        public DbSet<Advertising> Advertising { get; set; }     
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<ServicePlanItemStatuses> ServicePlanItemStatuses { get; set; }
        public DbSet<ClientsSubscribes> ClientsSubscribes { get; set; }
        public DbSet<SubscribesLog> SubscribesLog { get; set; }
        public DbSet<SmsCenter> SmsCenter { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<LocationAppointment> LocationAppointment { get; set; }
        public DbSet<ShedulerStatuses> ShedulerStatuses { get; set; }
        public DbSet<ResourceEntity> Resources { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatus { get; set; }

        public DbSet<SubscribeParams> SubscribeParams { get; set; }
        public DbSet<StatusSubscribe> StatusSubscribe { get; set; }
        public DbSet<UserActions> UserActions { get; set; }
        public DbSet<NotificationsLog> NotificationsLog { get; set; }     
    }
}
