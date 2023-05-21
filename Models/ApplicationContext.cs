using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Presentation;
using B6CRM.Models.Base;
using B6CRM.Services;
using System;
using Npgsql;
using B6CRM.Views.WindowForms;
using DevExpress.Xpf.Core;
using System.Windows;
using B6CRM.ViewModels;
using DevExpress.Data.Browsing;

namespace B6CRM.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            try
            {
                Config = new Config();
                Database.EnsureCreated();
            }
            catch
            {
            }
        }

        public ApplicationContext(Config config)
        {
            Config = config; ;
            Database.EnsureCreated();
        }

        public Config Config { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Config.DbType == 0) optionsBuilder.UseSqlite(@"Data Source=" + Config.ConnectionString);
            if (Config.DbType == 1) optionsBuilder.UseNpgsql(Config.ConnectionString);
            //Database.EnsureCreated();
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region AppointmentStatus seeding
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 1, Guid = "6z2yTR4DlGWqAJUxGzhC", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FF9BBB59", Caption = "Записан" });
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 2, Guid = "uxsU5kzxeGpEVCAeS3kd", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FF587816", Caption = "Подтвержден" });
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 3, Guid = "WjqwQSno8iB3bu3xFAvB", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FF83B5F1", Caption = "Пришел" });
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 4, Guid = "B8O9WyGSvY1bcJ99QoSy", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FF42A3BD", Caption = "В кресле" });
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 5, Guid = "dTjPfOje61wvfbJBaSKG", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FF533775", Caption = "Прием окончен" });
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 6, Guid = "ZK3JotTNIqMf4DHtw8g4", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FFE57572", Caption = "Отменен" });
            modelBuilder.Entity<AppointmentStatus>().HasData(new AppointmentStatus { Id = 7, Guid = "4tbcAEgO2oFQQRGpAdHW", CreatedAt = 1649579905, UpdatedAt = 1649579905, BrushColor = "#FFDC143C", Caption = "Отсутствие врача" });
            #endregion

            #region Branh seeding
            modelBuilder.Entity<Branch>().HasData(new Branch { Id = 1, Guid = "Yqugor9kOnCwMc1hW2zY", CreatedAt = 1649579905, UpdatedAt = 1649579905, WorkTime = "09:00:00-17:00:00" });
            #endregion

            #region PlanStatus seeding
            modelBuilder.Entity<PlanStatus>().HasData(new PlanStatus { Id = 1, Guid = "6z2yTR4DlGWqAJUxGz1U", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Принят", Sort = 1 });
            modelBuilder.Entity<PlanStatus>().HasData(new PlanStatus { Id = 2, Guid = "6z2yTR4DlGWqAJUxGzXf", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Отклонен", Sort = 2 });
            modelBuilder.Entity<PlanStatus>().HasData(new PlanStatus { Id = 3, Guid = "6z2yTR4DlGWqAJUxGzef", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Обсуждается", Sort = 3 });
            modelBuilder.Entity<PlanStatus>().HasData(new PlanStatus { Id = 4, Guid = "6z2yTR4DlGWqAJUxGzSW", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Завершен", Sort = 4 });
            modelBuilder.Entity<PlanStatus>().HasData(new PlanStatus { Id = 5, Guid = "6z2yTR4DlGWqAJUxGzRk", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Нет", Sort = 5 });
            #endregion

            #region RoleManagment seeding
            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 1, Guid = "BiGLr7B4fEm8z3iQJYWx", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Sheduler", PageTitle = "Расписание", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 1 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 2, Guid = "BiGLr7B4fEm8z3iQJYW1", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Clients", PageTitle = "Клиенты", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 2 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 3, Guid = "BiGLr7B4fEm8z3iQJYWz", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Employees", PageTitle = "Сотрудники", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 3 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 4, Guid = "BiGLr7B4fEm8z3iQJYWc", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Prices", PageTitle = "Прайс", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 4 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 5, Guid = "BiGLr7B4fEm8z3iQJYW9", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Documents", PageTitle = "Документы", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 5 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 6, Guid = "BiGLr7B4fEm8z3iQJYWb", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Statistics", PageTitle = "Статистика", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 9 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 7, Guid = "BiGLr7B4fEm8z3iQJ100", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Settings", PageTitle = "Настройки", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 0, IsCategory = 1, Num = 10 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 8, Guid = "BiGLr7B4fEm8z3iQJ101", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowSheduler", PageTitle = "Отображать в меню \"Расписание\"", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 1, IsCategory = 0, Num = 11 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 9, Guid = "BiGLr7B4fEm8z3iQJ102", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "AppointmentEditable", PageTitle = "Добавление и редактирование визитов", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 1, IsCategory = 0, Num = 12 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 10, Guid = "BiGLr7B4fEm8z3iQJ103", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "AppointmentDeletable", PageTitle = "Удаление визитов", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 1, IsCategory = 0, Num = 13 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 11, Guid = "BiGLr7B4fEm8z3iQJ104", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintSheduler", PageTitle = "Печать расписания", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 1, IsCategory = 0, Num = 14 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 12, Guid = "BiGLr7B4fEm8z3iQJ105", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShedulerStatusEditable", PageTitle = "Редактирование справочника \"Статусы\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 1, IsCategory = 0, Num = 15 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 13, Guid = "BiGLr7B4fEm8z3iQJ106", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShedulerLocationEditable", PageTitle = "Редактирование справочника \"Локации\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 1, IsCategory = 0, Num = 16 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 14, Guid = "BiGLr7B4fEm8z3iQJ107", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShedulerWorkTimeEditable", PageTitle = "Редактирование справочника \"Рабочее время\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 1, IsCategory = 0, Num = 17 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 15, Guid = "BiGLr7B4fEm8z3iQJ108", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowClients", PageTitle = "Отображать в меню \"Клиенты\"", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 18 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 16, Guid = "BiGLr7B4fEm8z3iQJ109", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ClientsEditable", PageTitle = "Добавление и редактирование клиентов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 19 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 17, Guid = "BiGLr7B4fEm8z3iQJ110", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ClientsDelitable", PageTitle = "Удаление клиентов", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 2, IsCategory = 0, Num = 20 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 18, Guid = "BiGLr7B4fEm8z3iQJ111", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "InvoiceEditable", PageTitle = "Добавление и редактирование счетов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 21 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 19, Guid = "BiGLr7B4fEm8z3iQJ112", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "InvoiceDelitable", PageTitle = "Удаление счетов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 22 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 20, Guid = "BiGLr7B4fEm8z3iQJ113", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintInvoice", PageTitle = "Печать счетов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 23 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 21, Guid = "BiGLr7B4fEm8z3iQJ114", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PlanEditable", PageTitle = "Добавление и редактирование планов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 24 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 22, Guid = "BiGLr7B4fEm8z3iQJ115", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PlanDelitable", PageTitle = "Удаление планов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 25 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 23, Guid = "BiGLr7B4fEm8z3iQJ116", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintPlan", PageTitle = "Печать планов", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 26 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 24, Guid = "BiGLr7B4fEm8z3iQJ117", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ClientsImport", PageTitle = "Импорт клиентов и сотрудников", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 2, IsCategory = 0, Num = 27 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 25, Guid = "BiGLr7B4fEm8z3iQJ118", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ClientsAddFieldsEditable", PageTitle = "Редактирование справочника \"Дополнительные поля\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 2, IsCategory = 0, Num = 28 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 26, Guid = "BiGLr7B4fEm8z3iQJ119", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ClientsCategoryEditable", PageTitle = "Редактирование справочника \"Категории клиентов\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 2, IsCategory = 0, Num = 29 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 27, Guid = "BiGLr7B4fEm8z3iQJ120", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ClientsAdvertisingEditable", PageTitle = "Редактирование справочника \"Рекламные источники\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 2, IsCategory = 0, Num = 30 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 28, Guid = "BiGLr7B4fEm8z3iQJ121", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowEmployees", PageTitle = "Отображать в меню \"Сотрудники\"", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 3, IsCategory = 0, Num = 31 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 29, Guid = "BiGLr7B4fEm8z3iQJ122", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "EmployeeEditable", PageTitle = "Добавление и редактирование", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 3, IsCategory = 0, Num = 32 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 30, Guid = "BiGLr7B4fEm8z3iQJ123", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "EmployeeDelitable", PageTitle = "Удаление", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 3, IsCategory = 0, Num = 33 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 31, Guid = "BiGLr7B4fEm8z3iQJ124", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintEmployees", PageTitle = "Печать", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 3, IsCategory = 0, Num = 34 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 33, Guid = "BiGLr7B4fEm8z3iQJ126", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowPrices", PageTitle = "Отображать в меню \"Прайсы\"", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 4, IsCategory = 0, Num = 36 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 34, Guid = "BiGLr7B4fEm8z3iQJ127", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PriceEditable", PageTitle = "Добавление и редактирование", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 4, IsCategory = 0, Num = 37 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 35, Guid = "BiGLr7B4fEm8z3iQJ128", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PriceDelitable", PageTitle = "Удаление", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 4, IsCategory = 0, Num = 38 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 36, Guid = "BiGLr7B4fEm8z3iQJ129", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintPrices", PageTitle = "Печать", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 4, IsCategory = 0, Num = 39 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 37, Guid = "BiGLr7B4fEm8z3iQJ130", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowDocuments", PageTitle = "Отображать в меню \"Документы\"", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 5, IsCategory = 0, Num = 40 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 38, Guid = "BiGLr7B4fEm8z3iQJ131", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "DocumentImport", PageTitle = "Импорт", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 5, IsCategory = 0, Num = 41 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 39, Guid = "BiGLr7B4fEm8z3iQJ132", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "DocumentEditable", PageTitle = "Редактирование", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 5, IsCategory = 0, Num = 42 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 40, Guid = "BiGLr7B4fEm8z3iQJ133", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "DocumentDelitable", PageTitle = "Удаление", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 5, IsCategory = 0, Num = 43 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 41, Guid = "BiGLr7B4fEm8z3iQJ134", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintDocument", PageTitle = "Печать", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 5, IsCategory = 0, Num = 44 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 42, Guid = "BiGLr7B4fEm8z3iQJ135", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowStatistics", PageTitle = "Отображать в меню \"Статистика\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 6, IsCategory = 0, Num = 45 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 43, Guid = "BiGLr7B4fEm8z3iQJ136", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowSettings", PageTitle = "Отображать в меню \"Настройки\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 7, IsCategory = 0, Num = 46 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 44, Guid = "BiGLr7B4fEm8z3iQJ137", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "PrintClients", PageTitle = "Печать списка клиентов", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 2, IsCategory = 0, Num = 47 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 45, Guid = "BiGLr7B4fEm8z3iQJ138", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ImportData", PageTitle = "Импорт данных (клиенты, сотрудники, прайсы)", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 7, IsCategory = 0, Num = 48 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 46, Guid = "BiGLr7B4fEm8z3iQJ139", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ExportData", PageTitle = "Экспорт данных (клиенты, сотрудники, прайсы)", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 0, ParentId = 7, IsCategory = 0, Num = 49 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 47, Guid = "BiGLr7B4fEm8z3iQJ140", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "Sms", PageTitle = "Рассылки", AdminAccess = 1, DoctorAccess = 1, ReceptionAccess = 1, ParentId = 0, IsCategory = 1, Num = 6 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 48, Guid = "BiGLr7B4fEm8z3iQJ141", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "ShowSmsSenders", PageTitle = "Отображать в меню \"Рассылки\"", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 47, IsCategory = 0, Num = 50 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 49, Guid = "BiGLr7B4fEm8z3iQJ141", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "SmsEditable", PageTitle = "Создание и редактирование", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 47, IsCategory = 0, Num = 51 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 50, Guid = "BiGLr7B4fEm8z3iQJ142", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "SmsDelitable", PageTitle = "Удаление", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 47, IsCategory = 0, Num = 52 });

            modelBuilder.Entity<RoleManagment>().HasData(new RoleManagment { Id = 51, Guid = "BiGLr7B4fEm8z3iQJ143", CreatedAt = 1657035967, UpdatedAt = 1657035967, PageName = "SmsSending", PageTitle = "Рассылка сообщений", AdminAccess = 1, DoctorAccess = 0, ReceptionAccess = 1, ParentId = 47, IsCategory = 0, Num = 53 });

            #endregion

            #region NotificationEvent seeding
            modelBuilder.Entity<NotificationEvent>().HasData(new NotificationEvent { Id = 1, Guid = "v2GoZDjzAvMEfLtUo856", CreatedAt = 1649579905, UpdatedAt = 1649579905, EventName = "AppointmentAdd", Name = "Добавление встречи в расписание", IsNotify = 1 });

            modelBuilder.Entity<NotificationEvent>().HasData(new NotificationEvent { Id = 2, Guid = "v2GoZDjzAvMEfLtUo878", CreatedAt = 1649579905, UpdatedAt = 1649579905, EventName = "AppointmentEdit", Name = "Редактирование встречи в расписании", IsNotify = 1 });

            modelBuilder.Entity<NotificationEvent>().HasData(new NotificationEvent { Id = 3, Guid = "v2GoZDjzAvMEfLtUo885", CreatedAt = 1649579905, UpdatedAt = 1649579905, EventName = "AppointmentRemove", Name = "Удаление встречи из расписания", IsNotify = 1 });
            #endregion

            #region Advertising seeding
            modelBuilder.Entity<Advertising>().HasData(new Advertising { Id = 1, Guid = "IUAoR5QCigKWIXeysqlM", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Сайт" });

            modelBuilder.Entity<Advertising>().HasData(new Advertising { Id = 2, Guid = "qtLajHsaXHw91X6aSS4F", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Объявление в газете" });

            modelBuilder.Entity<Advertising>().HasData(new Advertising { Id = 3, Guid = "NtpaSw3731XxujWFBOu8", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Рекомендация" });

            modelBuilder.Entity<Advertising>().HasData(new Advertising { Id = 4, Guid = "COtAMyo7RpYJ8xBlPprv", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Реклама на Тв" });

            modelBuilder.Entity<Advertising>().HasData(new Advertising { Id = 5, Guid = "D3vXEotJm4idVbsCFuC9", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Реклама на радио" });
            #endregion

            #region ClientCategories seeding
            modelBuilder.Entity<ClientCategory>().HasData(new ClientCategory { Id = 1, Guid = "v2GoZDjzAvMEfLtUo8qQ", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "VIP" });
            modelBuilder.Entity<ClientCategory>().HasData(new ClientCategory { Id = 2, Guid = "6cziEa1lUNVKx3IVvQXM", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Постоянные" });
            modelBuilder.Entity<ClientCategory>().HasData(new ClientCategory { Id = 3, Guid = "H09EIcOqv2lBaQQGiMo7", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Лояльные" });
            #endregion

            #region TemplateType seeding
            modelBuilder.Entity<TemplateType>().HasData(new TemplateType { Id = 1, Guid = "cgU3QCLX6S897DQX8XWB", CreatedAt = 1649579905, UpdatedAt = 1649579905, SysName = "string", CaptionRu = "Строка" });

            modelBuilder.Entity<TemplateType>().HasData(new TemplateType { Id = 2, Guid = "cgU3QCLX6S897DQX8X001", CreatedAt = 1649579905, UpdatedAt = 1649579905, SysName = "int", CaptionRu = "Целое число" });

            modelBuilder.Entity<TemplateType>().HasData(new TemplateType { Id = 3, Guid = "cgU3QCLX6S897DQX80002", CreatedAt = 1649579905, UpdatedAt = 1649579905, SysName = "float", CaptionRu = "Дробное число" });

            modelBuilder.Entity<TemplateType>().HasData(new TemplateType { Id = 4, Guid = "cgU3QCLX6S897DQX8X361", CreatedAt = 1649579905, UpdatedAt = 1649579905, SysName = "money", CaptionRu = "Денежное значение" });

            modelBuilder.Entity<TemplateType>().HasData(new TemplateType { Id = 5, Guid = "cgU3QCLX6S897DQX8X123", CreatedAt = 1649579905, UpdatedAt = 1649579905, SysName = "date", CaptionRu = "Дата" });

            modelBuilder.Entity<TemplateType>().HasData(new TemplateType { Id = 6, Guid = "cgU3QCLX6S897DQX8X365", CreatedAt = 1649579905, UpdatedAt = 1649579905, SysName = "datetime", CaptionRu = "Дата и время" });
            #endregion

            #region LocationAppointment seeding
            modelBuilder.Entity<LocationAppointment>().HasData(new LocationAppointment { Id = 1, Guid = "t9PSqESnLBRJRqsjuXyi", CreatedAt = 1641309027, UpdatedAt = 1641309027, Name = "Кабинет №1", Address = "Клиника \"Все свои\"" });
            #endregion

            #region Settings seeding
            modelBuilder.Entity<Setting>().HasData(new Setting { Id = 1, Guid = "pBZj3LMOqO0XZSU9X2GQ", CreatedAt = 1649579905, UpdatedAt = 1649579905, RolesEnabled = 0, IsPasswordRequired = 0 });
            #endregion

            #region Services seeding
            modelBuilder.Entity<Service>().HasData(new Service { Id = 1, Guid = "gCrPxVN8H8LtQRUrgSzD", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Услуги", ParentID = null, IsDir = 1, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 2, Guid = "I2WZnYUvs1VVVtdBVUzu", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Первичный/Повторный прием", ParentID = 1, IsDir = 1, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 4, Guid = "NiHheTpwMmSxnTDIKOT2", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Прием (осмотр, консультация) врача-стоматолога повторный", ParentID = 2, IsDir = 0, Code = "В01.065.008", Price = 300, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 5, Guid = "33JydkU04mNgFyfndSuQ", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Профилактический прием (осмотр, консультация) врача-стоматолога", ParentID = 2, IsDir = 0, Code = "В04.065.006", Price = 500, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 6, Guid = "MbCoOb3s4KfsUzFThTOO", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Сбор анамнеза и жалоб при патологии полости рта", ParentID = 2, IsDir = 0, Code = "A01.07.001", Price = 700, IsHidden = 0 });


            modelBuilder.Entity<Service>().HasData(new Service { Id = 7, Guid = "sqCB9LhDHRXNxqgq2w3y", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Материалы", IsDir = 1, IsHidden = 0, Sort = 2 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 8, Guid = "yf36A8CxbW0tZ3HwsqY3", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Перевязочный материал", ParentID = 7, IsDir = 1, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 9, Guid = "hwXfWzV5hY4fNxQx0ViN", CreatedAt = 1649579905, UpdatedAt = 1649579905, Code = "A123456", Price = 50, ParentID = 8, Name = "Бинт", IsDir = 0, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 10, Guid = "08hPzUGFIZTMt6B1hzfR", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Хирургическая стоматология и имплантология", ParentID = 1, IsDir = 1, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 11, Guid = "DUu7uohDX6xGpILJ3y1y", CreatedAt = 1649579905, UpdatedAt = 1649579905, Code = "B01.067.001", Price = 500, ParentID = 10, Name = "Прием (осмотр, консультация) врача-стоматолога-хирурга первичный", IsDir = 0, IsHidden = 0 });

            modelBuilder.Entity<Service>().HasData(new Service { Id = 12, Guid = "cuzdxHL7IqJEo1oJd41x", CreatedAt = 1649579905, UpdatedAt = 1649579905, Code = "A16.07.001", Price = 1000, ParentID = 10, Name = "Удаление зуба", IsDir = 0, IsHidden = 0 });

            #endregion

            #region CommonValues seeding
            modelBuilder.Entity<CommonValue>().HasData(new CommonValue { Id = 1, Guid = "uJZc1myLOfV8g41GsrK6", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Дата выдачи лицензии", SysName = "LicenseDate", Value = "12 июня 2020" });
            #endregion

            #region Employees seeding
            modelBuilder.Entity<Employee>().HasData(new Employee { Id = 1, Guid = "10lpNjEgmxmH7WywH9Ms", CreatedAt = 1649579905, UpdatedAt = 1649579905, FirstName = "Иван", LastName = "Светлицын", MiddleName = "Иванович", Email = "asvet@ya.ru", Phone = "(987) 454-5454", Post = "Хирург", IsInArchive = 0, IsInSheduler = 1, IsAdmin = 1, IsDoctor = 1, Telegram = "12345678900", IsNotify = 1 });
            #endregion

            #region AdditionalClientFields seeding
            modelBuilder.Entity<AdditionalClientField>().HasData(new AdditionalClientField { Id = 1, Guid = "9ztZxGWXODzLIAzSELqH", CreatedAt = 1649579905, UpdatedAt = 1649579905, TypeValueId = 2, Label = "Номер паспорта", SysName = "PassportID", Sort = 1 });
            #endregion

            #region Clients seeding
            modelBuilder.Entity<Client>().HasData(new Client { Id = 1, Guid = "tcDqq9mpQjhbuCstsk3w", CreatedAt = 1649579905, UpdatedAt = 1649579905, FirstName = "Александр", LastName = "Алейников", MiddleName = "Иванович", BirthDate = "02.06.1981", Gender = "Мужчина", Phone = "+7(987) 652-6622", Address = "г. Балаково, ул. Набережная Леонова, д. 85, кв. 55", IsInArchive = 0, Email = "aleinikov@ymail.com", ClientCategoryId = 1 });
            #endregion

            #region AdditionalClientValues seeding
            modelBuilder.Entity<AdditionalClientValue>().HasData(new AdditionalClientValue { Id = 1, Guid = "BeHqlf2wGa0QumnjleFx", CreatedAt = 1649579905, UpdatedAt = 1649579905, ClientId = 1, AdditionalFieldId = 1, Value = "854556" });
            #endregion

            #region Invoices seeding
            modelBuilder.Entity<Invoice>().HasData(new Invoice { Id = 1, Guid = "UAkxL0HhM3L6NyUEuhZ2", CreatedAt = 1649579905, UpdatedAt = 1649579905, Date = "14.06.2022 11:00:00", Number = "00000001", ClientId = 1, Paid = 1, EmployeeId = 1, DateTimestamp = 1655190000, AdvertisingId = 1 });
            #endregion

            #region Plans seeding
            modelBuilder.Entity<Plan>().HasData(new Plan { Id = 1, Guid = "iFbMPNzNLNvbZuJHKSLz", CreatedAt = 1649579905, UpdatedAt = 1649579905, Date = "10.03.2023 15:05:58", DateTimestamp = 1678446358, Name = "За лечение", IsMovedToInvoice = 0, ClientId = 1 });
            #endregion

            #region InvoiceItems seeding
            modelBuilder.Entity<InvoiceItems>().HasData(new InvoiceItems { Id = 1, Guid = "p2LTaLDRxunzKuLb1J3u", CreatedAt = 1649579905, UpdatedAt = 1649579905, Count = 1, InvoiceId = 1, Code = "В01.065.008", Name = "Прием (осмотр, консультация) врача-стоматолога повторный", Price = 300M });

            modelBuilder.Entity<InvoiceItems>().HasData(new InvoiceItems { Id = 2, Guid = "xHsPY1xZnW2y4waz6E2g", CreatedAt = 1649579905, UpdatedAt = 1649579905, Count = 1, InvoiceId = 1, Code = "A01.07.001", Name = "Сбор анамнеза и жалоб при патологии полости рта", Price = 700M });

            modelBuilder.Entity<InvoiceItems>().HasData(new InvoiceItems { Id = 3, Guid = "QLddnX2TJBqBBkUMCjVl", CreatedAt = 1649579905, UpdatedAt = 1649579905, Count = 2, InvoiceId = 1, Code = "A123456", Name = "Бинт", Price = 50M });
            #endregion

            #region Channels seeding
            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 1, Guid = "IUAoR5QCigKWIXeyDsRT", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Sms", ProstoSms = 1, SmsCenter = 1});

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 2, Guid = "IUAoR5QCigKWIXeyDsIK", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Email", ProstoSms = 0, SmsCenter = 1 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 3, Guid = "IUAoR5QCigKWIXeyDsWa", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "WhatsApp", ProstoSms = 1, SmsCenter = 0 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 4, Guid = "IUAoR5QCigKWIXeyDs01", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Viber", ProstoSms = 0, SmsCenter = 1 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 5, Guid = "IUAoR5QCigKWIXeyDs02", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Telegram", ProstoSms = 1, SmsCenter = 0 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 6, Guid = "IUAoR5QCigKWIXeyDs03", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "VK", ProstoSms = 1, SmsCenter = 1 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 7, Guid = "IUAoR5QCigKWIXeyDs04", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "OK", ProstoSms = 0, SmsCenter = 1 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 8, Guid = "IUAoR5QCigKWIXeyDs05", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Голосовые рассылки", ProstoSms = 0, SmsCenter = 1 });

            modelBuilder.Entity<Channel>().HasData(new Channel { Id = 9, Guid = "IUAoR5QCigKWIXeyDs06", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Mms", ProstoSms = 0, SmsCenter = 1 });
            #endregion

            #region SendingStatuses seeding
            modelBuilder.Entity<SendingStatus>().HasData(new SendingStatus { Id = 1, Guid = "TmbXjWKTyJ3hNPzpU04Y", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Ожидает отправки" });

            modelBuilder.Entity<SendingStatus>().HasData(new SendingStatus { Id = 2, Guid = "TmbXjWKTyJ3hNPzpU09K", CreatedAt = 1649579905, UpdatedAt = 1649579905, Name = "Отправлено" });
            #endregion

            #region CascadeRouting seeding
            modelBuilder.Entity<CascadeRouting>().HasData(new CascadeRouting { Id = 1, Guid = "WjqwQSno8iB3bu3xFA11", CreatedAt = 1649579905, UpdatedAt = 1649579905, Channel = "СМС", ProviderId = 1, Abbr = "sms", Num = 1 });

            modelBuilder.Entity<CascadeRouting>().HasData(new CascadeRouting { Id = 2, Guid = "WjqwQSno8iB3bu3xFAuP", CreatedAt = 1649579905, UpdatedAt = 1649579905, Channel = "Telegram", ProviderId = 1, Abbr = "tg", Num = 2 });

            modelBuilder.Entity<CascadeRouting>().HasData(new CascadeRouting { Id = 3, Guid = "WjqwQSno8iB3bu3xFArf", CreatedAt = 1649579905, UpdatedAt = 1649579905, Channel = "Viber", ProviderId = 1, Abbr = "vb", Num = 3 });

            modelBuilder.Entity<CascadeRouting>().HasData(new CascadeRouting { Id = 4, Guid = "WjqwQSno8iB3bu3xFA12", CreatedAt = 1649579905, UpdatedAt = 1649579905, Channel = "ВКонтакте", ProviderId = 1, Abbr = "vk", Num = 4 });

            modelBuilder.Entity<CascadeRouting>().HasData(new CascadeRouting { Id = 5, Guid = "WjqwQSno8iB3bu3xFA87", CreatedAt = 1649579905, UpdatedAt = 1649579905, Channel = "WhatsApp", ProviderId = 1, Abbr = "wp", Num = 5 });

            #endregion
        }

        public DbSet<Employee> Employes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<LocationAppointment> LocationAppointment { get; set; }
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

        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanStatus> PlanStatuses { get; set; }
        public DbSet<PlanItem> PlanItems { get; set; }

        public DbSet<TelegramNotification> TelegramNotifications { get; set; }
        public DbSet<NotificationEvent> NotificationEvents { get; set; }
        public DbSet<TelegramBot> TelegramBots { get; set; }

        public DbSet<Sms> Sms { get; set; }
        public DbSet<SmsRecipient> SmsRecipients { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<SendingStatus> SendingStatuses { get; set; }
        public DbSet<ServicePass> ServicesPasses { get; set; }
        public DbSet<CascadeRouting> CascadeRouting { get; set; }
        public DbSet<SmsSendingDate> SmsSendingDate { get; set; }
    }
}
