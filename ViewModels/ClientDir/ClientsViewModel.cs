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
                LoadClients();
                LoadInvoices();
                LoadEmployees();
                LoadPrintConditions();
                Model = new Client();

                Init(Model);

                ClientCategories = db.ClientCategories?.ToObservableCollection() ?? new ObservableCollection<ClientCategory>();
                Prices = db.Services.Where(f => f.IsHidden != 1)?.OrderBy(f => f.Sort).ToArray();
                Advertisings = db.Advertising.ToObservableCollection();
                PlanStatuses = db.PlanStatuses.OrderBy(f => f.Sort).ToArray();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список клиентов!", true);
            }
        }

        #region Права на выполнение команд

        public bool CanOpenDirectory(object p) => Model?.Id != 0;
        public bool CanExecuteFile(object p) => Model?.Id != 0;

        public bool CanCreate() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;
        public bool CanDelete() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsDelitable;
        public bool CanSave() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;

        public bool CanAddInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanDeleteInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceDelitable;

        public bool CanAddInvoiceItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanDeleteInvoiceItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanPrintInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PrintInvoice;

        public bool CanAddPlan(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanSavePlan() => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanDeletePlan(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanDelitable;

        public bool CanAddPlanItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanDeletePlanItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanMovedToInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanPrintPlan(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PrintPlan;

        public bool CanOpenFormFields() => ((UserSession)Application.Current.Resources["UserSession"]).ClientsAddFieldsEditable;
        public bool CanPrintClients() => ((UserSession)Application.Current.Resources["UserSession"]).PrintClients;

        public bool CanAttachmentFile(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;
        public bool CanDeleteFile(object p) => ((UserSession)Application.Current.Resources["UserSession"]).ClientsEditable;
        #endregion

        //это поле для привязки (используется в команде импорта данных)
        public ApplicationContext Db { get; set; }

        #region Загрузка списков клиентов и всех инвойсов 
        public void LoadClients(int? isArhive = 0)
        {
            Clients = db.Clients.Where(f => f.IsInArchive == isArhive).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Client>();
        }

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

        public void LoadPlans()
        {
            Plans = db.Plans?.Where(f => f.ClientId == Model.Id)?.
                Include(f => f.Client).
                Include(f => f.PlanStatus).
                Include(f => f.PlanItems).
                OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Plan>();
        }

        public ObservableCollection<Client> Clients
        {
            get { return GetProperty(() => Clients); }
            set { SetProperty(() => Clients, value); }
        }

        public ObservableCollection<Invoice> Invoices
        {
            get { return GetProperty(() => Invoices); }
            set { SetProperty(() => Invoices, value); }
        }

        public ObservableCollection<Invoice> ClientInvoices
        {
            get { return GetProperty(() => ClientInvoices); }
            set { SetProperty(() => ClientInvoices, value); }
        }

        public ObservableCollection<Employee> Employees
        {
            get { return GetProperty(() => Employees); }
            set { SetProperty(() => Employees, value); }
        }

        public ObservableCollection<ClientCategory> ClientCategories
        {
            get { return GetProperty(() => ClientCategories); }
            set { SetProperty(() => ClientCategories, value); }
        }

        public ObservableCollection<Advertising> Advertisings
        {
            get { return GetProperty(() => Advertisings); }
            set { SetProperty(() => Advertisings, value); }
        }

        public ObservableCollection<Plan> Plans
        {
            get { return GetProperty(() => Plans); }
            set { SetProperty(() => Plans, value); }
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
                    Init(Model);
                    if (Application.Current.Resources["Router"] is MainViewModel nav &&
                          nav?.NavigationService is NavigationServiceBase service &&
                          service.Current is PatientsList page
                          ) page.clientCard.tabs.SelectedIndex = 0;
                }

                if (p is Invoice invoice && invoice.Client != null)
                {
                    if (invoice.Client == null) return;
                    Model = invoice.Client;
                    Init(invoice.Client);

                    if (Application.Current.Resources["Router"] is MainViewModel nav &&
                     nav?.NavigationService is NavigationServiceBase service &&
                     service.Current is PatientsList page
                     )
                    {
                        page.clientCard.tabs.SelectedIndex = 1;
                        if (page.invoicesList.grid.ItemsSource is ObservableCollection<Invoice> invoices)
                        {
                            page.clientCard.Invoices.grid.SelectedItem = invoice;
                        }
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

        #region Работа с фильтрами и поиском в списке клиентов
        [Command]
        public void ShowArchive()
        {
            try
            {
                IsArchiveList = !IsArchiveList;
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        public bool IsArchiveList
        {
            get { return GetProperty(() => IsArchiveList); }
            set { SetProperty(() => IsArchiveList, value); }
        }

        public object LastNameSearch { get; set; }

        [Command]
        public void ClientsSearch()
        {
            try
            {
                Clients = db.Clients.Where(f => f.IsInArchive == (IsArchiveList ? 1 : 0)).OrderBy(f => f.LastName).ToObservableCollection();

                if (!string.IsNullOrEmpty(LastNameSearch?.ToString()))
                {
                    Clients = Clients.Where(f => f.LastName.ToLower().Contains(LastNameSearch.ToString().ToLower())).OrderBy(f => f.LastName).ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        #endregion

        #region Работа с разделом карты "Административная"
        private void Init(Client model)
        {
            try
            {
                ClientInfoViewModel = new ClientInfoViewModel(model);
                IsReadOnly = model?.Id != 0;

                // загружаем инвойсы и дневники отдельного клиента

                ClientInvoices = model?.Id != 0 ? db.Invoices?.Where(f => f.ClientId == model.Id)?.Include(f => f.Employee)?.Include(f => f.Client)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt)?.ToObservableCollection() : new ObservableCollection<Invoice>();
                // сбрасываем фильтр счетов в вкарте клиента на значение по умолчание
                ShowPaid = null;


                // загружаем встречи для вкладки "Посещения"
                Appointments = db.Appointments.Where(f => f.ClientInfoId == model.Id).Include(f => f.Employee).Include(f => f.Service).OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Appointments>();

                FieldsViewModel = new FieldsViewModel(model, db);
                EventChangeReadOnly += FieldsViewModel.ChangedReadOnly;

                FieldsViewModel.IsReadOnly = true;

                SetTabVisibility(FieldsViewModel.AdditionalFieldsVisible);
                PathToUserFiles = Path.Combine(Config.PathToFilesDirectory, Model?.Guid);
                Files = Directory.Exists(PathToUserFiles) ? new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection() : new ObservableCollection<FileInfo>();

                LoadPlans();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion


        #region Работа с разделом карты "Счета"
        #region Счета
        [Command]
        public void AddInvoice(object p)
        {
            try
            {
                if (p is Client client)
                {
                    var date = DateTime.Now;
                    var model = new Invoice
                    {
                        Number = NewInvoiceNumberGenerate(),
                        Date = date.ToString(),
                        DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                        Client = client,
                        ClientId = client?.Id
                    };

                    db.Invoices.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        ClientInvoices.Add(model);
                        LoadInvoices();
                        if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeleteInvoice(object p)
        {
            try
            {
                if (p is Invoice invoice)
                {
                    if (invoice.Id > 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить счет?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return;

                        invoice.InvoiceItems = null;
                        db.InvoiceItems.Where(f => f.InvoiceId == invoice.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                        db.Entry(invoice).State = EntityState.Deleted;

                    }
                    else
                    {
                        db.Entry(invoice).State = EntityState.Detached;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "Счет удален из базы данных!" }.run();
                    }

                    // удаляем из списков в карте и в общем списке счетов
                    // может не оказаться этого эл-та в списке, например, он в другом статусе
                    var inv = Invoices.FirstOrDefault(f => f.Guid == invoice.Guid);
                    if (inv != null) Invoices.Remove(inv);

                    var clientInv = ClientInvoices.FirstOrDefault(f => f.Guid == invoice.Guid);
                    if (clientInv != null) ClientInvoices.Remove(clientInv);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке удалить счет из базы данных!", true);
            }
        }

        private string NewInvoiceNumberGenerate()
        {
            if (int.TryParse(db.Invoices?.ToList()?.OrderByDescending(f => f.Id)?.FirstOrDefault()?.Number, out int invoicesNumber) &&
                int.TryParse(ClientInvoices.LastOrDefault()?.Number, out int clientInvoicesNumber))
            {
                if (clientInvoicesNumber > invoicesNumber) return string.Format("{0:00000000}", ++clientInvoicesNumber);
                return string.Format("{0:00000000}", ++invoicesNumber);
            }
            return "00000001";
        }
        #endregion

        #region Позиция в смете
        public object[] Prices { get; set; }

        [Command]
        public void SelectItemInField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.CurrentItem is Service service)
                    {
                        if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                        if (((GridCellData)parameters.Popup.DataContext).Row is InvoiceItems item)
                        {
                            item.Price = service.Price;
                            item.Code = service.Code;
                        }
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void AddInvoiceItem(object p)
        {
            try
            {
                if (p is Invoice invoice)
                {
                    invoice.InvoiceItems.Add(new InvoiceItems() { Invoice = invoice, InvoiceId = invoice?.Id });
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeleteInvoiceItem(object p)
        {
            try
            {
                if (p is InvoiceItems item)
                {
                    var items = item.Invoice.InvoiceItems;
                    if (item.Id > 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить позицию в счете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return;
                        item.Invoice = null;
                        db.Entry(item).State = EntityState.Deleted;
                        items.Remove(item);
                        db.SaveChanges();
                        new Notification() { Content = "Позиция удалена из счета!" }.run();
                        return;
                    }
                    db.Entry(item).State = EntityState.Detached;
                    items.Remove(item);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

        #region Печать счета
        [Command]
        public void PrintInvoice(object p)
        {
            try
            {
                if (p is PageIntCommandParameters conv)
                {
                    ServicesInvoiceReport report = new ServicesInvoiceReport();
                    var parameter = new Parameter()
                    {
                        Name = "Id",
                        Description = "Id:",
                        Type = typeof(int),
                        Value = conv.Param,
                        Visible = false
                    };
                    report.RequestParameters = false;
                    report.Parameters.Add(parameter);
                    report.FilterString = "[Id] = [Parameters.Id]";
                    report.Parameters["parameter_logo"].Value = Config.GetPathToLogo();

                    if (report?.DataSource is SqlDataSource source)
                    {
                        string connectionString = db.Database.GetConnectionString();
                        var provider = "XpoProvider=SQLite;";
                        if (Config.DbType == 1)
                        {
                            // connectionString = "Server=127.0.0.1;Port=5433;User ID=postgres;Password=657913;Database=B6Crm;Encoding=UNICODE";
                            provider = "XpoProvider=Postgres;";
                        }
                        source.ConnectionParameters = new CustomStringConnectionParameters(provider + connectionString);
                    }

                    PrintHelper.ShowPrintPreview(conv.Page, report);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при загрузке счета на печать!", true);
            }
        }
        #endregion

        #region Работа с фильтрами во вкладке "Счета" в карте клиента
        // фильтр показывать оплаченные/неоплаченные счета
        [Command]
        public void StatusChanged(object p)
        {
            try
            {
                if (int.TryParse(p?.ToString(), out int param))
                {
                    ShowPaid = param;
                    if (param == -1)
                    {
                        ShowPaid = null;
                        ClientInvoices = db.Invoices?.Where(f => f.ClientId == Model.Id)?.Include(f => f.Employee)?.Include(f => f.Client)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt)?.ToObservableCollection();
                        return;
                    }

                    ClientInvoices = db.Invoices?.Where(f => f.ClientId == Model.Id && f.Paid == ShowPaid)?.Include(f => f.Employee)?.Include(f => f.Client)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt)?.ToObservableCollection();
                    return;
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

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
        #endregion


        #region Раздел карты "Дополнительные поля"
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
                Log.ErrorHandler(e, "При открытии формы \"Дополнительные поля\" возникла ошибка!", true);
            }
        }

        public void UpdateFields()
        {
            FieldsViewModel.ClientFieldsLoading(Model);
            AdditionalFieldsVisible = FieldsViewModel?.Fields.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion


        #region Карта клиента

        [Command]
        public void Create()
        {
            try
            {
                Model = new Client();
                Init(Model);
                SelectedItem();

                if (Application.Current.Resources["Router"] is MainViewModel nav &&
                    nav?.NavigationService is NavigationServiceBase service &&
                    service.Current is PatientsList page
                    ) page.clientCard.tabs.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Editable()
        {
            IsReadOnly = !IsReadOnly;
            EventChangeReadOnly?.Invoke(IsReadOnly || Model?.Id == 0);
        }

        [Command]
        public void Save()
        {
            try
            {
                ClientInfoViewModel.Copy(Model);

                /************************/
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
                /************************/

                if (Model.Id == 0) // новый элемент
                {
                    db.Clients.Add(Model);
                    // если статус анкеты (в архиве или нет) не отличается от текущего статуса списка, то тогда добавить
                    if (IsArchiveList == (Model.IsInArchive == 1)) Clients?.Insert(0, Model);
                    db.SaveChanges();
                    EventChangeReadOnly?.Invoke(false); // разблокировать дополнительные поля
                                                        //EventNewClientSaved?.Invoke(Model); // разблокировать команды счетов
                    PathToUserFiles = Path.Combine(Config.PathToFilesDirectory, Model?.Guid);
                    new Notification() { Content = "Новый клиент успешно записан в базу данных!" }.run();
                    SelectedItem();
                }
                else
                { // редактирование су-щего эл-та
                    FieldsViewModel?.Save(Model);
                    if (db.SaveChanges() > 0)
                    {
                        LoadClients(Model.IsInArchive);
                        IsArchiveList = Model.IsInArchive == 1;
                        SelectedItem();
                        new Notification() { Content = "Отредактированные данные сохранены в базу данных!" }.run();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При сохранении данных возникла ошибка!", true);
            }
        }

        [Command]
        public void Delete()
        {
            try
            {
                var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить карту клиента из базы данных, без возможности восстановления? Также будут удалены счета, записи в расписании и все файлы прикрепленные к карте клиента!",
                messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                if (response.ToString() == "No") return;

                new UserFilesManagement(Model.Guid).DeleteDirectory();
                var id = Model?.Id;

                //удалить также в расписании и в счетах
                db.Appointments.Where(f => f.ClientInfoId == Model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Invoices.Include(f => f.InvoiceItems).Where(f => f.ClientId == Model.Id).ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.AdditionalClientValue.Where(f => f.ClientId == Model.Id)?.ForEach(f => db.Entry(f).State = EntityState.Deleted);

                db.Entry(Model).State = EntityState.Deleted;
                if (db.SaveChanges() > 0) new Notification() { Content = "Карта клиента полностью удалена из базы данных!" }.run();

                // может не оказаться этого эл-та в списке, например, он в статусе "В архиве"
                var item = Clients.FirstOrDefault(f => f.Guid == Model.Guid);
                if (item != null) Clients.Remove(item);

                db.InvoiceItems.Where(f => f.InvoiceId == null).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                db.SaveChanges();


                // удаляем файлы 
                if (Directory.Exists(PathToUserFiles)) Directory.Delete(PathToUserFiles);

                //загружаем новую анкету
                Model = new Client();
                Init(Model);
                SelectedItem();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При удалении карты клиента произошла ошибка!", true);
            }
        }

        [Command]
        public void ClearDate(object p)
        {
            if (p is DateEdit field)
            {
                field.ClearError();
                field.Clear();
                field.ClosePopup();
                field.EditValue = null;
                ClientInfoViewModel.BirthDate = null;
            }
        }

        public Client Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public ClientInfoViewModel ClientInfoViewModel
        {
            get { return GetProperty(() => ClientInfoViewModel); }
            set { SetProperty(() => ClientInfoViewModel, value); }
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


        #region команды, связанных с прикреплением файлов        
        public string PathToUserFiles
        {
            get { return GetProperty(() => PathToUserFiles); }
            set { SetProperty(() => PathToUserFiles, value); }
        }

        public ObservableCollection<FileInfo> Files
        {
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }

        [Command]
        public void OpenDirectory(object p)
        {
            try
            {
                if (PathToUserFiles != null && Directory.Exists(PathToUserFiles))
                {
                    var proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(PathToUserFiles)
                    {
                        UseShellExecute = true
                    };
                    proc.Start();
                }

                //Process.Start(PathToUserFiles);
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Невозможно открыть содержащую файл директорию!", true);
            }
        }

        [Command]
        public void ExecuteFile(object p)
        {
            try
            {
                if (p is FileInfo file)
                {
                    var proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(file.FullName)
                    {
                        UseShellExecute = true
                    };
                    proc.Start();
                }
                //Process.Start(file.FullName);
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Невозможно выполнить загрузку файла!", true);
            }
        }

        [Command]
        public void AttachmentFile(object p)
        {
            try
            {
                var filePath = string.Empty;
                using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    dialog.InitialDirectory = "c:\\";
                    dialog.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
                    dialog.FilterIndex = 2;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                    filePath = dialog.FileName;
                    if (string.IsNullOrEmpty(filePath)) return;
                }

                FileInfo file = new FileInfo(filePath);

                // проверяем на наличие существующего файла
                foreach (var i in Files)
                {
                    if (string.Compare(i.Name, file.Name, StringComparison.CurrentCulture) == 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Файл с таким именем уже есть в списке прикрепленных файлов. Заменить текущий файл?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return; // не захотел, поэтому дальше ничего не делаем

                        // Решил заменить файл, удаляем файл, добавляем новый и перезагружаем коллекцию
                        i.Delete();
                    }
                }

                if (PathToUserFiles != null && !Directory.Exists(PathToUserFiles)) Directory.CreateDirectory(PathToUserFiles);



                File.Copy(file.FullName, Path.Combine(PathToUserFiles, file.Name), true);
                File.SetAttributes(Path.Combine(PathToUserFiles, file.Name), FileAttributes.Normal);

                FileInfo newFile = new FileInfo(Path.Combine(PathToUserFiles, file.Name));
                newFile.CreationTime = DateTime.Now;

                Files = new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeleteFile(object p)
        {
            try
            {
                if (p is FileInfo file)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить файл с компьютера?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    File.SetAttributes(file.FullName, FileAttributes.Normal);
                    file.Delete();
                    Files = new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
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


        #region Планы клиента
        public ICollection<PlanStatus> PlanStatuses { get; private set; }

        #region Планы работ
        [Command]
        public void AddPlan(object p)
        {
            try
            {
                if (p is Client client)
                {
                    var date = DateTime.Now;
                    PlanStatus status = db.PlanStatuses.FirstOrDefault(s => s.Id == 5);

                    var model = new Plan
                    {
                        Date = date.ToString(),
                        DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                        Client = client,
                        ClientId = client?.Id,
                        PlanStatus = status,
                        PlanStatusId = status?.Id
                    };

                    db.Add(model);
                    Plans.Add(model);
                }
                if (db.SaveChanges() > 0) new Notification() { Content = "Добавлен новый план в базу данных!" }.run();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void SavePlan()
        {
            try
            {
                #region Lic
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
                #endregion

                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке сохранить план в базе данных!", true);
            }
        }

        [Command]
        public void DeletePlan(object p)
        {
            try
            {
                if (p is Plan plan)
                {
                    if (plan.Id > 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить план?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return;

                        plan.PlanItems = null;
                        db.PlanItems.Where(f => f.PlanId == plan.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                        db.Entry(plan).State = EntityState.Deleted;

                    }
                    else
                    {
                        db.Entry(plan).State = EntityState.Detached;
                    }
                    if (db.SaveChanges() > 0)
                    {
                        new Notification() { Content = "План удален из базы данных!" }.run();
                    }

                    // удаляем из списков в карте и в общем списке счетов
                    // может не оказаться этого эл-та в списке, например, он в другом статусе
                    var inv = Plans.FirstOrDefault(f => f.Guid == plan.Guid);
                    if (inv != null) Plans.Remove(inv);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке удалить план из базы данных!", true);
            }
        }

        #endregion

        #region Позиция в плане

        [Command]
        public void SelectPlanItemInField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.CurrentItem is Service service)
                    {
                        if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                        if (((GridCellData)parameters.Popup.DataContext).Row is PlanItem item)
                        {
                            item.Price = service.Price;
                            item.Code = service.Code;
                        }
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void AddPlanItem(object p)
        {
            try
            {
                if (p is Plan plan)
                {
                    plan.PlanItems.Add(new PlanItem() { Plan = plan, PlanId = plan?.Id });
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void DeletePlanItem(object p)
        {
            try
            {
                if (p is PlanItem item)
                {
                    var items = item.Plan.PlanItems;
                    if (item.Id > 0)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить позицию в плане?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                        if (response.ToString() == "No") return;
                        item.Plan = null;
                        db.Entry(item).State = EntityState.Deleted;
                        items.Remove(item);
                        db.SaveChanges();
                        new Notification() { Content = "Позиция удалена из плана!" }.run();
                        return;
                    }
                    db.Entry(item).State = EntityState.Detached;
                    items.Remove(item);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void ShedulerOpening(object p)
        {
            try
            {
                new ShedulerWindow().ShowDialog();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void MovedToInvoice(object p)
        {
            try
            {
                if (p is Plan plan)
                {
                    List<PlanItem> items = new List<PlanItem>();

                    foreach (var item in plan.PlanItems)
                    {
                        if (item.IsInInvoice == 1 && item.IsMovedToInvoice != 1)
                        {
                            item.IsMovedToInvoice = 1;
                            items.Add(item);
                        }
                    }

                    if (items.Count > 0)
                    {
                        var date = DateTime.Now;
                        var invoice = new Invoice
                        {
                            Number = NewInvoiceNumberGenerate(),
                            Date = date.ToString(),
                            DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                            Client = Model,
                            ClientId = Model?.Id
                        };

                        items.ForEach(f => invoice.InvoiceItems.Add(new InvoiceItems()
                        {
                            Code = f.Code,
                            Count = f.Count,
                            Price = f.Price,
                            Name = f.Name,
                            Invoice = invoice
                        }));

                        db.Invoices.Add(invoice);
                        if (db.SaveChanges() > 0)
                        {
                            ClientInvoices.Add(invoice);
                            LoadInvoices();
                            new Notification() { Content = $"Сформирован новый счет №{invoice.Number}" }.run();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        #endregion

        [Command]
        public void PrintPlan(object p)
        {
            try
            {
                if (p is PageIntCommandParameters conv)
                {
                    Report2 report = new Report2();
                    var parameter = new Parameter()
                    {
                        Name = "Id",
                        Description = "Id:",
                        Type = typeof(int),
                        Value = conv.Param,
                        Visible = false
                    };
                    report.RequestParameters = false;
                    report.Parameters.Add(parameter);
                    report.FilterString = "[Id] = [Parameters.Id]";
                    //report.Parameters["parameter_logo"].Value = Config.GetPathToLogo();

                    if (report?.DataSource is SqlDataSource source)
                    {
                        string connectionString = db.Database.GetConnectionString();
                        var provider = "XpoProvider=SQLite;";
                        if (Config.DbType == 1) provider = "XpoProvider=Postgres;";
                        source.ConnectionParameters = new CustomStringConnectionParameters(provider + connectionString);
                    }

                    PrintHelper.ShowPrintPreview(conv.Page, report);
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при загрузке счета на печать!", true);
            }
        }
        #endregion

        #region Печать
        public ObservableCollection<PrintCondition> PrintConditions
        {
            get { return GetProperty(() => PrintConditions); }
            set { SetProperty(() => PrintConditions, value); }
        }

        public object PrintConditionsSelected { get; set; }

        private void LoadPrintConditions()
        {
            PrintConditions = new ObservableCollection<PrintCondition>()
            {
                new PrintCondition(){Name = "В архиве", Id = -2, Type = true.GetType()}
            };
            db.ClientCategories?.ToArray()?.ForEach(f => PrintConditions.Add(
                new PrintCondition() { Name = f.Name, Id = f.Id, Type = f.GetType() }
                ));
        }

        [Command]
        public void PrintClients()
        {
            PrintClientsWindow = new PrintClientsWindow() { DataContext = this };
            PrintClientsWindow.Show();
        }

        [Command]
        public void LoadDocForPrint()
        {
            try
            {
                // Create a link and assign a data source to it.
                // Assign your data templates to different report areas.
                CollectionViewLink link = new CollectionViewLink();
                CollectionViewSource Source = new CollectionViewSource();

                SetSourceCollectttion();

                Source.Source = SourceCollection;

                Source.GroupDescriptions.Add(new PropertyGroupDescription("ClientCategory.Name"));

                link.CollectionView = Source.View;
                link.GroupInfos.Add(new GroupInfo((DataTemplate)PrintClientsWindow.Resources["CategoryTemplate"]));
                link.DetailTemplate = (DataTemplate)PrintClientsWindow.Resources["ProductTemplate"];

                // Associate the link with the Document Preview control.
                PrintClientsWindow.preview.DocumentSource = link;

                // Generate the report document 
                // and show pages as soon as they are created.
                link.CreateDocument(true);
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }

        }

        public ICollection<Client> SourceCollection { get; set; } = new List<Client>();


        private void SetSourceCollectttion()
        {
            try
            {
                SourceCollection = new List<Client>();
                var ctx = db.Clients;
                var where = "";

                if (PrintConditionsSelected is List<object> collection)
                {
                    var marked = collection.OfType<PrintCondition>().ToArray();

                    if (marked.FirstOrDefault(f => f.Id == -2) != null) where += " WHERE IsInArchive = 1";
                    //ctx.Where(f => f.IsInArchive == true);

                    var cat = marked.Where(f => f.Type == new ClientCategory().GetType())?.Select(f => f.Id)?.OfType<int?>().ToArray();

                    if (cat.Length > 0)
                    {
                        where += !string.IsNullOrEmpty(where) ? " OR" : " WHERE";
                        where += $" ClientCategoryId IN ({string.Join(",", cat)}) ";
                    }
                }

                if (!string.IsNullOrEmpty(where))
                {
                    SourceCollection = db.Clients.FromSqlRaw("SELECT * FROM ClientInfo" + where).
                       Include(f => f.ClientCategory).
                       OrderBy(f => f.ClientCategoryId).
                       ThenBy(f => f.LastName).
                       ToArray();
                    return;
                }
                SourceCollection = db.Clients.
                   Include(f => f.ClientCategory).
                   OrderBy(f => f.ClientCategoryId).
                   ThenBy(f => f.LastName).
                   ToArray();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        public PrintClientsWindow PrintClientsWindow { get; set; }

        #endregion
    }

}