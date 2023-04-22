using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using B6CRM.Views.PatientCard;
using B6CRM.Views.WindowForms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using DevExpress.Mvvm.Native;
using B6CRM.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using B6CRM.Views.Documents;
using B6CRM.Services.Files;
using B6CRM.Views.AdditionalFields;
using B6CRM.ViewModels.AdditionalFields;
using System.IO;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models.Base;
using DevExpress.Xpf.Editors;
using System.Diagnostics;
using B6CRM.Reports;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Parameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using System.Text.Json;
using DevExpress.Xpf.Grid;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI.Navigation;
using License;
using System.Windows.Data;
using Microsoft.VisualBasic;
using GroupInfo = DevExpress.Xpf.Printing.GroupInfo;
using System.Linq.Expressions;
using B6CRM.Models;
using B6CRM.Services;
using B6CRM.Infrastructures.Converters;

namespace B6CRM.ViewModels.ClientDir
{
    public class ClientsViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public delegate void ChangeReadOnly(bool status);
        public event ChangeReadOnly EventChangeReadOnly;

        public ClientsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Db = db;
                Config = db.Config;
                LoadInvoices();
                LoadEmployees();
                
                Model = new Client();

                //ClientCategoriesLoad();
                //Prices = db.Services.Where(f => f.IsHidden != 1)?.OrderBy(f => f.Sort).ToArray();
                AdvertisingLoad();
                
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список клиентов!", true);
            }
        }

        #region Загружаем справочники

        public void AdvertisingLoad() => Advertisings = db.Advertising.ToObservableCollection();

        //public void ClientCategoriesDeleteOrSave() { ClientCategoriesLoad(); LoadClients(); }

        public void AdvertisingDeleteOrSave() { }
        #endregion

        #region Права на выполнение команд
        public bool CanOpenFormFields() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsAddFieldsEditable;
        public bool CanPrintClients() => ((UserSession)Application.Current.Resources["UserSession"]).PrintClients;
        #endregion

        //это поле для привязки (используется в команде импорта данных)
        public ApplicationContext Db { get; set; }

        #region Загрузка списков клиентов и всех инвойсов 

        public void LoadInvoices()
        {
            // общие инвойсы
            Invoices = db.Invoices?.
                Include(f => f.Employee).
                Include(f => f.Client).
                Include(f => f.InvoiceItems).
                OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Invoice>();
        }

        public void LoadEmployees()
        {
            Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Employee>();
            foreach (var i in Employees) i.IsVisible = false;
        }

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

        #endregion

        #region Переход из списка инвойсов или списков клиентов (загрузка карты)

        [Command]
        public void Load(object p)
        {
            try
            {
                if (p is Client model)
                {
                    Model = model;
                  /*  if (Application.Current.Resources["Router"] is MainViewModel nav &&
                          nav?.NavigationService is NavigationServiceBase service &&
                          service.Current is PatientsList page
                          ) page.clientCard.tabs.SelectedIndex = 0;*/
                }

                if (p is Invoice invoice && invoice.Client != null)
                {
                    if (invoice.Client == null) return;
                    Model = invoice.Client;

                    if (Application.Current.Resources["Router"] is MainViewModel nav &&
                     nav?.NavigationService is NavigationServiceBase service &&
                     service.Current is PatientsList page
                     )
                    {
                      /*  page.clientCard.tabs.SelectedIndex = 1;
                        if (page.invoicesList.grid.ItemsSource is ObservableCollection<Invoice> invoices)
                        {
                            page.clientCard.Invoices.grid.SelectedItem = invoice;
                        }*/
                    }
                }

                if (Status.Licensed && Status.HardwareID != Status.License_HardwareID)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (!Status.Licensed && Status.Evaluation_Time_Current > Status.Evaluation_Time)
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

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

                if (Application.Current.Resources["Router"] is MainViewModel nav &&
                     nav?.NavigationService is NavigationServiceBase service &&
                     service.Current is PatientsList page
                     )
                {
                    page.SelectedInvoiceItem();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

 

        #region Работа с разделом карты "Административная"
      
        #endregion


        #region Работа с разделом карты "Счета"

        #region Работа с фильтрами во вкладке "Счета" в карте клиента


        public int? ShowPaid
        {
            get { return GetProperty(() => ShowPaid); }
            set { SetProperty(() => ShowPaid, value); }
        }
        #endregion
        #endregion

        #region Работа с разделом карты "Дополнительные поля"
        public ObservableCollection<Appointments> Appointments
        {
            get { return GetProperty(() => Appointments); }
            set { SetProperty(() => Appointments, value); }
        }

        [Command]
        public void OpenFormFields()
        {
            try
            {
                var vm = new AdditionalClientFieldsViewModel(db);
                vm.EventFieldChanges += UpdateFields;
                new ClientFieldsWindow() { DataContext = vm }?.Show();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При открытии формы \"Дополнительные поля\" произошла ошибка!", true);
            }
        }

        public void UpdateFields()
        {
            FieldsViewModel.ClientFieldsLoading(Model);
            AdditionalFieldsVisible = FieldsViewModel?.Fields.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        [Command]
        public void Editable()
        {
            IsReadOnly = !IsReadOnly;
            EventChangeReadOnly?.Invoke(IsReadOnly || Model?.Id == 0);
        }

        #region Карта клиента

 

        public Client Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        public Visibility AdditionalFieldsVisible
        {
            get { return GetProperty(() => AdditionalFieldsVisible); }
            set { SetProperty(() => AdditionalFieldsVisible, value); }
        }

        public FieldsViewModel FieldsViewModel
        {
            get { return GetProperty(() => FieldsViewModel); }
            set { SetProperty(() => FieldsViewModel, value); }
        }

        public void SetTabVisibility(Visibility visibility) => AdditionalFieldsVisible = visibility;
        #endregion



        private void SelectedItem()
        {
            if (Application.Current.Resources["Router"] is MainViewModel nav &&
                     nav?.NavigationService is NavigationServiceBase service &&
                     service.Current is PatientsList page
                     ) page.SelectedItem();
        }

        private INavigationService NavigationService { get { return GetService<INavigationService>(); } }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }


        public void ClientInvoicesUpdate(int id)
        {
            //ClientInvoices.Where(f => f.AdvertisingId == id).ForEach(f => f.AdvertisingId = null);
            db.SaveChanges();
        }
    }

}