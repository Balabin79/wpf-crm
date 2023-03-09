using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Views.PatientCard;
using Dental.Views.WindowForms;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Services;
using Dental.Models;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Documents;
using Dental.Services.Files;
using Dental.Views.AdditionalFields;
using Dental.ViewModels.AdditionalFields;
using System.IO;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Models.Base;
using DevExpress.Xpf.Editors;
using System.Diagnostics;
using Dental.Infrastructures.Converters;
using Dental.Reports;
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

namespace Dental.ViewModels.ClientDir
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
                Config = db.Config;
                LoadClients();
                LoadInvoices();
                LoadEmployees();
                Model = new Client();

                Init(Model);

                ClientCategories = db.ClientCategories?.ToObservableCollection() ?? new ObservableCollection<ClientCategory>();
                Prices = db.Services.Where(f => f.IsHidden != true)?.OrderBy(f => f.Sort).ToArray();
                Advertisings = db.Advertising.ToObservableCollection();
            }
            catch (Exception e)
            {
               ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка подключения к базе данных при попытке загрузить список клиентов!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #region Права на выполнение команд

        public bool OpenDirectory() => Model?.Id != 0;
        public bool ExecuteFile() => Model?.Id != 0;
        public bool AttachmentFile() => Model?.Id != 0;
        public bool DeleteFile() => Model?.Id != 0;
        //public bool CanShowArchive() => true;
        #endregion

        #region Загрузка списков клиентов и всех инвойсов 
        public void LoadClients(bool isArhive = false)
        {        
            Clients = db.Clients.Where(f => f.IsInArchive == isArhive).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Client>();
        }

        public void LoadInvoices()
        {
            // общие инвойсы
            Invoices = db.Invoices?.Include(f => f.Employee).Include(f => f.Client).OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Invoice>();
        }

        public void LoadEmployees()
        {
            Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Employee>();
            foreach (var i in Employees) i.IsVisible = false;        
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
                        page.clientCard.tabs.SelectedIndex = 2;
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
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            catch { }
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
                (new ViewModelLog(e)).run();
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
                Clients = db.Clients.Where(f => f.IsInArchive == IsArchiveList).OrderBy(f => f.LastName).ToObservableCollection();

                if (!string.IsNullOrEmpty(LastNameSearch?.ToString()))
                {
                    Clients = Clients.Where(f => f.LastName.ToLower().Contains(LastNameSearch.ToString().ToLower())).OrderBy(f => f.LastName).ToObservableCollection();                  
                }
            }
            catch (Exception e)
            {

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
            }
            catch (Exception e)
            {

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
                    ClientInvoices.Add(new Invoice
                    {
                        Number = NewInvoiceNumberGenerate(),
                        Date = date.ToString(),
                        DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                        Client = client,
                        ClientId = client?.Id
                    });
                }
            }
            catch (Exception e)
            {

            }
        }

        [Command]
        public void SaveInvoice()
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
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                #endregion

                foreach(var invoice in ClientInvoices.ToList())
                {
                    if (invoice.Id == 0)
                    {
                        db.Entry(invoice).State = EntityState.Added;
                        Invoices?.Add(invoice);
                    }

                    if (invoice.InvoiceItems?.Count > 0)
                    {
                        var items = invoice.InvoiceItems;

                        foreach (var i in items.ToList())
                        {
                            if (string.IsNullOrEmpty(i.Name))
                            {
                                i.Invoice = null;
                                db.Entry(i).State = EntityState.Detached;
                                invoice.InvoiceItems.Remove(i);
                                continue;
                            }
                            if (i.Id == 0) { db.Entry(i).State = EntityState.Added; }
                        }
                    }
                }

                if (db.SaveChanges() > 0) new Notification() { Content = "Изменения сохранены в базу данных!" }.run();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке сохранить счет в базе данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить счет из базы данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
            catch
            {

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
            catch(Exception e)
            {

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
                (new ViewModelLog(e)).run();
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
                        if (Config.DbType == 1) provider = "XpoProvider=Postgres;";
                        source.ConnectionParameters = new CustomStringConnectionParameters(provider + connectionString);
                    }

                    PrintHelper.ShowPrintPreview(conv.Page, report);
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка!", text: "Ошибка при загрузке счета на печать!", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Error);
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
            catch { }
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
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Дополнительные поля\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public void UpdateFields() 
        { 
            FieldsViewModel.ClientFieldsLoading(Model);
            AdditionalFieldsVisible = FieldsViewModel?.Fields.Count > 0 ?  Visibility.Visible : Visibility.Collapsed;
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
            catch(Exception e) 
            { }
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
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
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
                    if (IsArchiveList == Model.IsInArchive) Clients?.Insert(0, Model);
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
                    if (db.SaveChanges() > 0) {
                        LoadClients((bool)Model.IsInArchive);
                        IsArchiveList = (bool)Model.IsInArchive;
                        SelectedItem();
                        new Notification() { Content = "Отредактированные данные клиента сохранены в базу данных!" }.run(); 
                    }                                  
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При сохранении данных клиента возникла ошибка!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                ThemedMessageBox.Show(title: "Ошибка", text: "При удалении карты клиента произошла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
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
                if (PathToUserFiles != null && Directory.Exists(PathToUserFiles)) Process.Start(PathToUserFiles);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                    text: "Невозможно открыть содержащую файл директорию!",
                    messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void ExecuteFile(object p)
        {
            try
            {
                if (p is FileInfo file) Process.Start(file.FullName);
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка",
                   text: "Невозможно выполнить загрузку файла!",
                   messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                (new ViewModelLog(e)).run();
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
                (new ViewModelLog(e)).run();
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
                (new ViewModelLog(e)).run();
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

        private INavigationService NavigationService { get { return this.GetService<INavigationService>(); } }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }
    }
}