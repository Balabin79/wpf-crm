using System;
using System.Collections.Generic;
using System.Linq;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class ReportsByEmpViewModel : ViewModelBase
    {

        public ReportsByEmpViewModel()
        {
            try
            {
                db = new ApplicationContext();
                ServicesItems = db.EstimateServiceItems.Include("Employee").Include("Services").ToArray();

                Employes = ServicesItems.GroupBy(f => f.EmployeeId).Select(i => new Employes
                {
                    Name = i.Select(f => f.Employee?.Fio).FirstOrDefault(),
                    Prices = i.Select(f => f.Service?.Price).Sum()
                }).ToList();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Действия пользователя\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public IEnumerable<EstimateServiceItem> ServicesItems { get; }
        public IEnumerable<Employes> Employes { get; }

        ApplicationContext db;
    }

    public class Employes
    {
        public decimal? Prices { get; set; }
        public decimal? Costs { get; set; }
        public string Name { get; set; }
    }
}