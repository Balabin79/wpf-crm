using CsvHelper;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Models;
using Dental.ViewModels;
using DevExpress.CodeParser;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot.Types;

namespace Dental.Services
{
    internal class ImportData : ViewModelBase
    {
        private readonly ApplicationContext db;
        private IEnumerable<object> records;

        public ImportData() => db = new ApplicationContext();

        public bool CanImport(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsImport;

        [Command]
        public void Import(object p)
        {
            try
            {
                var filePath = string.Empty;
                var fileName = string.Empty;
                int i = 0;
                string page = "";
                if(p is Type type)
                {
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
                        if (type == typeof(Client))
                        {
                            i = LoadClients(csv);
                            page = "Dental.Views.PatientCard.PatientsList";
                        } 
                            
                        if (type == typeof(Employee))
                        {
                            i = LoadStaff(csv);
                            page = "Dental.Views.Staff";
                        }

                        if (type == typeof(Service))
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
                string fileName = "";
                
                if (p is Type type && type != null)
                {
                    if (type == typeof(Employee))
                    {
                        fileName = "staff.csv";
                        records = db.Employes.OrderBy(f => f.LastName).ToList();
                    }
                    if (type == typeof(Client))
                    {
                        fileName = "clients.csv";
                        records = db.Clients.Include(f => f.ClientCategory).OrderBy(f => f.ClientCategoryId).ThenBy(f => f.LastName).ToList();
                    }
                    if (type == typeof(Service))
                    {
                        fileName = "prices.csv";
                        records = db.Services.OrderBy(f => f.ParentID).ThenBy(f => f.Name).ToList();
                    }

                    Unload(fileName);
                }

            } 
            catch(Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке экспортировать данные!", true);
            }
        }

        #region Загрузка данных
        private int LoadClients(CsvReader csv)
        {
            var records = csv.GetRecords<ClientExport>();
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
                    ClientCategoryId = item.ClientCategoryId
                };
                db.Clients.Add(model);
                i++;
            }       
            return i;
        }

        private int LoadStaff(CsvReader csv)
        {
            var records = csv.GetRecords<EmployeeExport>();
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
                    Post = item.Post
                };
                db.Employes.Add(model);
                i++;
            }
            return i;
        }

        private int LoadPrice(CsvReader csv)
        {
            var records = csv.GetRecords<PriceExport>();
            int i = 0;
            foreach (var item in records)
            {
                var model = new Service()
                {
                    Name = item.Name,
                    Code = item.Code,
                    Price  = item.Price,
                    IsDir = item.IsDir,
                    ParentID = item.ParentID
                };
                db.Services.Add(model);
                i++;
            }
            return i;
        }
        #endregion

        #region Выгрузка данных
        private void Unload(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (records is List<Employee> employees) csv.WriteRecords(employees);
                if (records is List<Client> clients) csv.WriteRecords(clients);
                if (records is List<Service> prices)
                {
                    csv.WriteHeader<PriceExport>();
                    csv.NextRecord();
                    foreach (var record in prices)
                    {
                        csv.WriteRecord(new PriceExport() { Id = record.Id, Name = record.Name, Code = record.Code, Price = record.Price, IsDir = record.IsDir, ParentID = record.ParentID});
                        csv.NextRecord();
                    }
                }                  
            }
            new Notification() { Content = $"Список успешно выгружен в файл {filePath}" }.run();
        }
        #endregion
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
