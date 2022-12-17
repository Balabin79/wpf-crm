using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dental.Models.Templates;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.Common;
using Dental.Services;
using System;
using System.IO;
using System.Data.SQLite;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite.EF6;
using System.Data.Entity.Core.Common;
using System.Data.SqlClient;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Views.Settings;
using Dental.ViewModels;

namespace Dental.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base(
            new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder()
                {
                    DataSource = Config.ConnectionString,
                    Version = 3
                }.ConnectionString
            }, true
            )
        {
            bool result = false;
            while (!result) result = CheckFileDbExists() && CheckConnectionToDb();           
        }

        private bool CheckConnectionToDb()
        {
            bool openDbSuccess = false;
            do
            {
                try
                {
                    using (var conn = new SQLiteConnection()
                    {
                        ConnectionString = new SQLiteConnectionStringBuilder()
                        {
                            DataSource = Config.ConnectionString,
                            Version = 3
                        }.ConnectionString
                    })
                    {

                        conn.Open();
                        openDbSuccess = true;
                    }
                }
                catch (Exception e)
                {
                    ThemedMessageBox.Show(
                        title: "Ошибка",
                        text: "Ошибка при попытке подключения к базе данных. Если используется локальная сеть, то, возможно, в данный момент недоступно сетевое расположение файла. Проверьте настройки подключения к базе данных.",
                        messageBoxButtons: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error, windowStartupLocation: WindowStartupLocation.CenterOwner
                        );

                    new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
                    continue;
                }
            }
            while (!openDbSuccess);
            return true;
        }

        private bool CheckFileDbExists()
        {
            while (!File.Exists(Config.ConnectionString))
            {
                ThemedMessageBox.Show(
                    title: "Ошибка",
                    text: "Не найден файл базы данных. Если используется локальная сеть, то, возможно, в данный момент недоступно сетевое расположение файла. Проверьте настройки подключения к базе данных.",
                    messageBoxButtons: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error, windowStartupLocation: WindowStartupLocation.CenterOwner
                    );

                new PathsSettingsWindow() { DataContext = new PathsSettingsVM() }?.ShowDialog();
            }
            return true;
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

        public DbSet<UserActions> UserActions { get; set; }
        public DbSet<Nomenclature> Nomenclature { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItems> InvoiceItems { get; set; }

        public DbSet<AdditionalClientField> AdditionalClientFields { get; set; }
        public DbSet<TemplateType> TemplateType { get; set; }
        public DbSet<CommonValue> CommonValues { get; set; }
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

        public DbSet<Organization> Organizations { get; set; }
    }
}
