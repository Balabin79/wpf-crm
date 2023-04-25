using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Views.PatientCard;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.WindowsUI.Navigation;
using License;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Payments;
using Invoice = B6CRM.Models.Invoice;

namespace B6CRM.ViewModels.ClientDir
{
    internal class InvoicesListViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public InvoicesListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                LoadInvoices();
                LoadEmployees();
                AdvertisingLoad();

                //SelectedClient = new Client();
                LoadClients();
                //LoadPrintConditions();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список планов работ!", true);
            }
        }

        public void LoadInvoices()
        {
            // общие инвойсы
            Invoices = db.Invoices?.
                Include(f => f.Employee).
                Include(f => f.Client).
                OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Invoice>();
        }

        public void LoadClients(int? isArhive = 0) =>
            Clients = db.Clients.Where(f => f.IsInArchive == isArhive).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Client>();

        public void LoadEmployees()
        {
            Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Employee>();
            foreach (var i in Employees) i.IsVisible = false;
        }

        public void AdvertisingLoad() => Advertisings = db.Advertising.ToObservableCollection();

        #region Работа с фильтрами и поиском в списке инвойсов
        public object EmployeeSearch { get; set; }
        public object ClientSearch { get; set; }
        public object DateFromSearch { get; set; }
        public object DateToSearch { get; set; }
        public object InvoiceNameSearch { get; set; }
        public object InvoicePaidSearch { get; set; }
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

        [Command]
        public void Search()
        {
            try
            {
                List<string> where = new List<string>();
                long dateFrom = new DateTimeOffset(new DateTime(1970, 1, 1)).ToUnixTimeSeconds();
                long dateTo = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                var date = DateTimeOffset.FromUnixTimeSeconds(dateTo).LocalDateTime;

                if (int.TryParse(ClientSearch?.ToString(), out int clientId) && clientId != 0) where.Add("ClientId=" + clientId.ToString());
                if (int.TryParse(EmployeeSearch?.ToString(), out int employeeId) && employeeId != 0) where.Add("EmployeeId=" + employeeId.ToString());

                if (int.TryParse(InvoicesSearchMode?.ToString(), out int paimentStatus))
                {
                    if (paimentStatus == 1) where.Add("Paid = 1");
                    if (paimentStatus == 2) where.Add("Paid = 0");
                }

                if (DateFromSearch != null && DateTime.TryParse(DateFromSearch?.ToString(), out DateTime dateTimeFrom))
                {
                    dateFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
                }

                if (DateToSearch != null && DateTime.TryParse(DateToSearch?.ToString(), out DateTime dateTimeTo))
                {
                    dateTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();
                }

                //DateTimeOffset.FromUnixTimeSeconds(dateFrom).LocalDateTime
                string parameters = "WHERE ";
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
                parameters += "DateTimestamp >= " + dateFrom + " AND DateTimestamp <= " + dateTo;

                //SqlParameter param = SqlParameter("@name", "%Samsung%");
                //var phones = db.Database.SqlQuery<Phone>("SELECT * FROM Phones WHERE Name LIKE @name", param);
                Invoices = db.Invoices.FromSqlRaw("SELECT * FROM Invoices " + parameters + " ORDER BY DateTimestamp DESC").ToObservableCollection();
                //Invoices = query?.Include(f => f.Client)?.Include(f => f.Employee)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt).ToObservableCollection();
                if (!string.IsNullOrEmpty(InvoiceNameSearch?.ToString()))
                {
                    Invoices = Invoices.Where(f => f.Number.Contains(InvoiceNameSearch?.ToString().ToLower())).OrderByDescending(f => f.DateTimestamp).ToObservableCollection();
                }

                /*if (Application.Current.Resources["Router"] is MainViewModel nav &&
                     nav?.NavigationService is NavigationServiceBase service &&
                     service.Current is PatientsList page
                     )
                {
                    page.SelectedInvoiceItem();
                }*/
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

        public ObservableCollection<Invoice> Invoices
        {
            get { return GetProperty(() => Invoices); }
            set { SetProperty(() => Invoices, value); }
        }

        public ObservableCollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }

        public ObservableCollection<Advertising> Advertisings
        {
            get { return GetProperty(() => Advertisings); }
            set { SetProperty(() => Advertisings, value); }
        }

        public ObservableCollection<Client> Clients
        {
            get { return GetProperty(() => Clients); }
            set { SetProperty(() => Clients, value); }
        }

    }
}
