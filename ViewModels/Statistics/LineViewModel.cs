using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Models.Base;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.Statistics
{
    public class LineViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public LineViewModel()
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

        public ObservableCollection<Series> Data
        {
            get { return GetProperty(() => Data); }
            set { SetProperty(() => Data, value); }
        }


        [Command]
        public void Search()
        {
            try
            {
                var all = db.Invoices.OrderBy(f => f.DateTimestamp).ToArray();
                if (all == null) return;

                Data = new ObservableCollection<Series>();

                var dateStart = !string.IsNullOrEmpty(DateFrom) ? DateFrom : all.First()?.Date;
                var dateEnd = !string.IsNullOrEmpty(DateTo) ? DateTo : all.Last()?.Date;


                AllEmployeeProfit = new List<EmployeeProfit>();
                List<string> where = new List<string>();

                if (int.TryParse(InvoicesSearchMode?.ToString(), out int paimentStatus))
                {
                    if (paimentStatus == 1) where.Add("Paid = 1");
                    if (paimentStatus == 2) where.Add("Paid = 0");
                }

                DateTime dateTimeFrom = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime dateTimeTo = DateTime.UtcNow;

                long dateFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
                long dateTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();

                var date = DateTimeOffset.FromUnixTimeSeconds(dateTo).LocalDateTime;


                if (dateStart != null && DateTime.TryParse(dateStart?.ToString(), out DateTime from))
                {
                    dateFrom = new DateTimeOffset(from).ToUnixTimeSeconds();
                    dateTimeFrom = from;
                }

                if (dateEnd != null && DateTime.TryParse(dateEnd?.ToString(), out DateTime to))
                {
                    dateTo = new DateTimeOffset(to).ToUnixTimeSeconds();
                    dateTimeTo = to;
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

                    var invoiceItems = db.InvoiceItems
                        .FromSqlRaw("SELECT * FROM InvoiceItems left join Invoices on Invoices.Id = InvoiceItems.InvoiceId "
                        + cond + " ORDER by DateTimestamp").Include(f => f.Invoice)
                        .ToList();

                    //заполняем периода от dateTimeFrom до dateTimeTo
                    var values = new List<Period>();
                    var iter = dateTimeFrom;
                    while (iter <= dateTimeTo)
                    {
                        values.Add(new Period() { PeriodName = iter.ToString("MMMM yyyy"), Sum = 0 });

                        iter = new DateTime(iter.Year, iter.Month, 1).AddMonths(1);
                    }

                    var seria = new Series() { Name = i.FullName, Values = values };

                    foreach (var item in invoiceItems)
                    {
                        if (DateTime.TryParse(item.Invoice?.Date, out DateTime dateTime))
                        {
                            string periodName = dateTime.ToString("MMMM yyyy");
                            var period = seria.Values.FirstOrDefault(f => f.PeriodName == periodName);

                            if (period != null)
                            {
                                period.Sum += item.Count * item.Price;
                            }
                        }
                    }
                    Data.Add(seria);
                }
                //SetDefaultPeriods();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        private void SetDefaultPeriods()
        {
            Data = new ObservableCollection<Series>();

        }

        public List<EmployeeProfit> AllEmployeeProfit
        {
            get { return GetProperty(() => AllEmployeeProfit); }
            set { SetProperty(() => AllEmployeeProfit, value); }
        }
    }

    public class EmployeeProfit : ViewModelBase
    {
        public string Fio
        {
            get { return GetProperty(() => Fio); }
            set { SetProperty(() => Fio, value); }
        }

        public decimal? Sum { get; set; } = 0;

        public string Period
        {
            get { return GetProperty(() => Period); }
            set { SetProperty(() => Period, value); }
        }
    }

    public class Series : ViewModelBase
    {
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public List<Period> Values
        {
            get { return GetProperty(() => Values); }
            set { SetProperty(() => Values, value); }
        }
    }

    public class Period
    {
        public string PeriodName { get; set; }
        public decimal? Sum { get; set; }
    }



}
