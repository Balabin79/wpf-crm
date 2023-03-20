using Dental.Models;
using Dental.Models.Base;
using Dental.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels.Statistics
{
    public class ProfitByEmployeesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ProfitByEmployeesViewModel()
        {
            db = new ApplicationContext();
            Employees = db.Employes.ToObservableCollection();
            Search();
        }


        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public ObservableCollection<Employee> Employees { get; private set; }
        public ICollection<object> EmployeesSearch { get; set; }

        public int? InvoicesSearchMode
        {
            get { return GetProperty(() => InvoicesSearchMode); }
            set { SetProperty(() => InvoicesSearchMode, value); }
        }

        [Command]
        public void SwitchInvoicesSearchMode(object p)
        {
            if (p == null) p = 0;
            if (int.TryParse(p.ToString(), out int param)) InvoicesSearchMode = param;
        }

        public ObservableCollection<object> Data
        {
            get { return GetProperty(() => Data); }
            set { SetProperty(() => Data, value); }
        }


        [Command]
        public void Search()
        {
            try
            {
                Data = new ObservableCollection<object>();
                long dateFrom = new DateTimeOffset(new DateTime(1970, 1, 1)).ToUnixTimeSeconds();
                long dateTo = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                int? paid = null;

                var date = DateTimeOffset.FromUnixTimeSeconds(dateTo).LocalDateTime;

                if (int.TryParse(InvoicesSearchMode?.ToString(), out int paimentStatus)) 
                {
                   if (paimentStatus == 1) paid = 1; 
                   if (paimentStatus == 2) paid = 0; 
                }         

                if (DateFrom != null && DateTime.TryParse(DateFrom?.ToString(), out DateTime dateTimeFrom))
                {
                    dateFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
                }

                if (DateTo != null && DateTime.TryParse(DateTo?.ToString(), out DateTime dateTimeTo))
                {
                    dateTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();
                }

                // если в фильтре EmployeesSearch указаны сотрудники, то используем доп. фильтр, иначе по всей коллекции (Employees)

                ICollection<Employee> employees;
                if (EmployeesSearch?.Count > 0) 
                    employees = EmployeesSearch.OfType<Employee>().ToArray(); 
                else employees = Employees;

                foreach (var i in employees)
                {

                    var where = (paid == null) ?
                        db.InvoiceItems
                        .Where(f => f.Count != 0 && f.Price != null && f.Invoice.DateTimestamp >= dateFrom && f.Invoice.DateTimestamp <= dateTo && f.Invoice.EmployeeId == i.Id)
                        .Include(f => f.Invoice) :

                        db.InvoiceItems
                        .Where(f => f.Count != 0 && f.Price != null && f.Invoice.Paid == paid && f.Invoice.DateTimestamp >= dateFrom && f.Invoice.DateTimestamp <= dateTo && f.Invoice.EmployeeId == i.Id)
                        .Include(f => f.Invoice);


                   var invoices = where.ToList();
                        

                    decimal? sum = 0.00M;
                    foreach(var inv in invoices)
                    {
                        sum += inv.Count * inv.Price;
                    }
                    Data.Add(new { Employee = i.FullName, Sum = sum });

                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }


            /*Data =
     new ObservableCollection<object> {
                    new  { Period = "Антон М.П.", Sum = (decimal?)715000.00},
                    new { Period = "Иванов И.А.", Sum = (decimal?)602000.00},
                    new  { Period = "Светлакова А.С.", Sum = (decimal?)631000.00},
                    new  { Period = "Светлицын И.И.", Sum = (decimal?)690000.00},
                    new  { Period = "Семушкина И.А.", Sum = (decimal?)210000.00},
                    new  { Period = "Федотова И.В.", Sum = (decimal?)426000.00},
                    new  { Period = "Фомин П.А.", Sum = (decimal?)47000.00}
       };*/
        }

    }
}
