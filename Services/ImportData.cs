using CsvHelper;
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Models;
using Dental.ViewModels;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace Dental.Services
{
    internal class ImportData : ViewModelBase
    {
        private ApplicationContext db;

        public ImportData() => db = new ApplicationContext();

        public bool CanImport(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ImportData;
        public bool CanExport(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ExportData;

        [Command]
        public void Import(object p)
        {
            try
            {
                if (p is ExportDataCommandParameters parameters)
                {
                    if (parameters.Type == null || parameters.Context == null) return;
                    var filePath = string.Empty;
                    var fileName = string.Empty;
                    int i = 0;
                    string page = "";
                    db = parameters.Context;

                    using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                    {
                        openFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

                        openFileDialog.Filter = "Files(*.csv) | *.csv";
                        openFileDialog.FilterIndex = 2;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) filePath = openFileDialog.FileName;
                        openFileDialog.Dispose();
                    }
                    if (string.IsNullOrEmpty(filePath)) return;

                    /// обработка csv
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        if (parameters.Type == typeof(Client))
                        {
                            i = LoadClients(csv);
                            page = "Dental.Views.PatientCard.PatientsList";
                        }

                        if (parameters.Type == typeof(Employee))
                        {
                            i = LoadStaff(csv);
                            page = "Dental.Views.Staff";
                        }

                        if (parameters.Type == typeof(Service))
                        {
                            i = LoadPrice(csv);
                            page = "Dental.Views.ServicePrice.ServicePage";
                        }

                        if (db.SaveChanges() > 0)
                        {
                            new Notification() { Content = $"Список успешно импортирован в базу данных! Добавлено {i} записей" }.run();

                            if (Application.Current?.Resources["Router"] is MainViewModel vm)
                            {
                                vm?.NavigationService?.Navigate(page, null);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке импортировать данные!", true);
            }
        }

        [Command]
        public void Export(object p)
        {
            try
            {
                if (p is ExportDataCommandParameters parameters)
                {
                    if (parameters.Type == null || parameters.Context == null) return;

                    db = parameters.Context;

                    if (parameters.Type == typeof(Employee))
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "staff.csv");
                        using (var writer = new StreamWriter(filePath))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var records = db.Employes.OrderBy(f => f.LastName).ToList();
                            csv.WriteHeader<EmployeeExport>();
                            csv.NextRecord();
                            foreach (var record in records)
                            {
                                csv.WriteRecord(new EmployeeExport()
                                {
                                    LastName = record.LastName,
                                    FirstName = record.FirstName,
                                    MiddleName = record.MiddleName,
                                    Phone = record.Phone,
                                    Email = record.Email,
                                    Telegram = record.Telegram,
                                    Post = record.Post,
                                    IsArchive = record.IsInArchive
                                });
                            }
                            Notify(filePath);
                        }
                    }

                    if (parameters.Type == typeof(Client))
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "clients.csv");
                        using (var writer = new StreamWriter(filePath))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var records = db.Clients.Include(f => f.ClientCategory).OrderBy(f => f.ClientCategoryId).ThenBy(f => f.LastName).ToList();
                            csv.WriteHeader<ClientExport>();
                            csv.NextRecord();
                            foreach (var record in records)
                            {
                                csv.WriteRecord(new ClientExport()
                                {
                                    LastName = record.LastName,
                                    FirstName = record.FirstName,
                                    MiddleName = record.MiddleName,
                                    BirthDate = record.BirthDate,
                                    Gender = record.Gender,
                                    Address = record.Address,
                                    Phone = record.Phone,
                                    Email = record.Email,
                                    ClientCategoryId = record.ClientCategoryId,
                                    ClientCategoryName = record.ClientCategory?.Name,
                                    IsArchive = record.IsInArchive
                                });
                            }
                            Notify(filePath);
                        }
                    }

                    if (parameters.Type == typeof(Service))
                    {
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "prices.csv");
                        using (var writer = new StreamWriter(filePath))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var records = db.Services.OrderBy(f => f.ParentID).ThenBy(f => f.Name).ToList();
                            csv.WriteHeader<PriceExport>();
                            csv.NextRecord();
                            foreach (var record in records)
                            {
                                csv.WriteRecord(new PriceExport() { Id = record.Id, Name = record.Name, Code = record.Code, Price = record.Price, IsDir = record.IsDir, ParentID = record.ParentID });
                                csv.NextRecord();
                            }
                        }
                        Notify(filePath);
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке экспортировать данные!", true);
            }
        }

        #region Загрузка данных
        private int LoadClients(CsvReader csv)
        {
            var records = csv.GetRecords<ClientExport>()?.OrderBy(f => f.LastName);
            int i = 0;
            foreach (var item in records)
            {
                var model = new Client()
                {
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    MiddleName = item.MiddleName,
                    BirthDate = item.BirthDate,
                    Gender = item.Gender,
                    Email = item.Email,
                    Phone = item.Phone,
                    Address = item.Address,
                    ClientCategoryId = item.ClientCategoryId,
                    IsInArchive = item.IsArchive
                };
                db.Clients.Add(model);
                i++;
            }
            return i;
        }

        private int LoadStaff(CsvReader csv)
        {
            var records = csv.GetRecords<EmployeeExport>()?.OrderBy(f => f.LastName);
            int i = 0;
            foreach (var item in records)
            {
                var model = new Employee()
                {
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    MiddleName = item.MiddleName,
                    Email = item.Email,
                    Phone = item.Phone,
                    Telegram = item.Telegram,
                    Post = item.Post,
                    IsInArchive = item.IsArchive
                };
                db.Employes.Add(model);
                i++;
            }
            return i;
        }

        private int LoadPrice(CsvReader csv)
        {
            var records = csv.GetRecords<PriceExport>()?.OrderBy(f => f.Id);
            int i = 0;
            foreach (var item in records)
            {
                var model = new Service()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code = item.Code,
                    Price = item.Price,
                    IsDir = item.IsDir,
                    ParentID = item.ParentID
                };
                db.Services.Add(model);
                i++;
            }
            return i;
        }
        #endregion

        private void Notify(string filePath) => new Notification() { Content = $"Список успешно выгружен в файл {filePath}" }.run();
    }

    internal class ClientExport
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ClientCategoryId { get; set; }
        public string ClientCategoryName { get; set; }
        public int? IsArchive { get; set; }
    }

    internal class EmployeeExport
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Telegram { get; set; }
        public string Post { get; set; }
        public int? IsArchive { get; set; }
    }

    internal class PriceExport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? Price { get; set; }
        public int? IsDir { get; set; }
        public int? ParentID { get; set; }
    }
}
