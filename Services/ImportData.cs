using CsvHelper;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Models;
using DevExpress.CodeParser;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    internal class ImportData : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ImportData() => db = new ApplicationContext();

        [Command]
        public void FromCsv(object p)
        {
            try
            {
                var clientsExport = new List<ClientExport>();
                // создаем файл для экспорта данных
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                       "clients_" + DateTime.Now.Ticks.ToString() + ".csv");
                //var file = File.Create(filePath);

                // выбираем данные
                var clients = db.Clients.Include(f => f.ClientCategory).OrderBy(f => f.LastName).ToArray();

                // преобразовываем данные
                clients.ForEach(f => clientsExport.Add(new ClientExport()
                {
                    LastName = f.LastName,
                    FirstName = f.FirstName,
                    MiddleName = f.MiddleName,
                    BirthDate = f.BirthDate,
                    Gender = f.Gender,
                    Address = f.Address,
                    Phone = f.Phone,
                    Email = f.Email,
                    ClientCategoryId = f.ClientCategoryId,
                    ClientCategoryName = f.ClientCategory?.Name,
                    IsArhive = f.IsInArchive == true ? "да" : "нет"
                }));

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(clientsExport);
                }
            }
            catch (Exception e)
            {

            }
        }



        [Command]
        public void Import(object p)
        {
            try
            {
                var filePath = string.Empty;
                var fileName = string.Empty;
                int i = 0;
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
                        if (type == typeof(Client)) i = LoadClients(csv);
                        if (type == typeof(Employee)) i = LoadStaff(csv);

                        if (db.SaveChanges() > 0)
                        {
                            new Notification() { Content = $"Список успешно импортирован в базу данных! Добавлено {i} записей" }.run();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке импортировать данные!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

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
                    ClientCategoryId = item.ClientCategoryId,
                    IsInArchive = item.IsArhive == "да" ? true : false
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
                    Post = item.Post,
                    IsInArchive = item.IsArhive == "да" ? true : false
                };
                db.Employes.Add(model);
                i++;
            }
            return i;
        }
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
        public string IsArhive { get; set; }
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
        public string IsArhive { get; set; }
    }
}
