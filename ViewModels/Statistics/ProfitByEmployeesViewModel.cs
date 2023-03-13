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
                List<string> where = new List<string>();
                long dateFrom = new DateTimeOffset(new DateTime(1970, 1, 1)).ToUnixTimeSeconds();
                long dateTo = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                var date = DateTimeOffset.FromUnixTimeSeconds(dateTo).LocalDateTime;

                //if (int.TryParse(EmployeeSearch?.ToString(), out int employeeId) && employeeId != 0) where.Add("EmployeeId=" + employeeId.ToString());

                if (int.TryParse(InvoicesSearchMode?.ToString(), out int paimentStatus))
                {
                    if (paimentStatus == 1) where.Add("Paid = 1");
                    if (paimentStatus == 2) where.Add("Paid = 0");
                }

                if (DateFrom != null && DateTime.TryParse(DateFrom?.ToString(), out DateTime dateTimeFrom))
                {
                    dateFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
                }

                if (DateTo != null && DateTime.TryParse(DateTo?.ToString(), out DateTime dateTimeTo))
                {
                    dateTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();
                }

                string parameters = "WHERE Count is not null and Price is not null and ";
                for (int i = 0; i < where.Count; i++)
                {
                    if (i == 0)
                    {
                        parameters += where[i];
                        continue;
                    }
                    parameters += " AND " + where[i];
                }
                if (where.Count > 0) parameters += " AND ";

                // если в фильтре EmployeesSearch указаны сотрудники, то используем доп. фильтр, иначе по всей коллекции (Employees)

                ICollection<Employee> employees;
                if (EmployeesSearch?.Count > 0) 
                    employees = EmployeesSearch.OfType<Employee>().ToArray(); 
                else employees = Employees;

                foreach (var i in employees)
                {
                    string cond = parameters;
                    cond += " EmployeeId = " + i?.Id + " AND DateTimestamp >= " + dateFrom + " AND DateTimestamp <= " + dateTo;

                    var invoices = db.InvoiceItems.FromSqlRaw("SELECT * FROM InvoiceItems left join Invoices on Invoices.Id = InvoiceItems.InvoiceId " + cond).ToList();

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
