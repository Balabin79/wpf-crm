using CsvHelper;
using Dental.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services
{
    internal class Export : ViewModelBase
    {
        private readonly ApplicationContext db;

        public Export() => db = new ApplicationContext();

        [Command]
        public void ToCsv(object p)
        {
            try
            {
                var clientsExport = new List<ClientExport>();
                // создаем файл для экспорта данных
                 string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "clients_"+ DateTime.Now.Ticks.ToString() + ".csv");
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
            catch(Exception e)
            {

            }
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
}
