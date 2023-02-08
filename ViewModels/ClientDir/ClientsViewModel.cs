using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Views.PatientCard;
using Dental.Views.WindowForms;
using System.Data.Entity;
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
using System.Windows.Media.Imaging;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.ViewModels.Invoices;
using Dental.Models.Base;
using Dental.Infrastructures.Extensions;
using DevExpress.Xpf.Editors;
using System.Diagnostics;
using System.Windows.Media;
using Dental.Views.Invoices;
using Dental.Infrastructures.Converters;
using Dental.Reports;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Parameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using System.Text.Json;
using DevExpress.Xpf.Grid;
using System.Text;
using System.Windows.Controls;
using DevExpress.Xpf.LayoutControl;
using System.Globalization;
using System.Windows.Data;
using Dental.Views.Settings;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI.Navigation;
using System.Reflection;
using License;
using Dental.Views.About;

namespace Dental.ViewModels.ClientDir
{
    public class ClientsViewModel : ViewModelBase, IImageDeletable, IImageSave
    {
        private readonly ApplicationContext db;

        public delegate void ChangeReadOnly(bool status);
        public event ChangeReadOnly EventChangeReadOnly;

        public ClientsViewModel()
        {
            try
            {

                db = new ApplicationContext();
                Config = new Config();
                LoadClients();
                LoadInvoices();
                LoadEmployees();
                Model = new Client();

                Init(Model);
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
            foreach (var i in Clients) ImgLoading(i);
        }

        public void LoadInvoices()
        {
            // общие инвойсы
            Invoices = db.Invoices?.Include(f => f.Employee).Include(f => f.Client).OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Invoice>();
        }

        public void LoadEmployees()
        {

            Employees = db.Employes.OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Employee>();
            foreach (var i in Employees)
            {
                ImgLoading(i);
                i.IsVisible = false;
            }
        }

        private void ImgLoading(AbstractUser model)
        {
            try
            {
                /*
                if (model.Img == null) return;
               
                using (var ms = new MemoryStream(model.Img))
                {
                    BitmapImage biImg = new BitmapImage();
                    biImg.BeginInit();
                    biImg.StreamSource = ms;
                    biImg.EndInit();



                   // model.Image = new BitmapImage() { StreamSource = ms};
       
                    model.Image = biImg;
                }*/
                      
                    var config = new Config();
                    var path = "";
                    if (model is Employee) path = Config.PathToEmployeesDirectory;

                    if (model is Client) 
                        path = Config.PathToClientsPhotoDirectory;

                    if (Directory.Exists(path))
                    {
                        var file = Directory.GetFiles(path)?.FirstOrDefault(f => f.Contains(model.Guid));
                        if (file == null) return;

                        using (var stream = new FileStream(file, FileMode.Open))
                        {
                            var img = new BitmapImage();
                            img.BeginInit();
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.StreamSource = stream;
                            img.EndInit();
                            img.Freeze();
                            model.Image = img;
                        }
                    }
                }
            catch
            {

            }
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
                Invoices = db.Invoices.SqlQuery("SELECT * FROM Invoices " + parameters + " ORDER BY DateTimestamp DESC").ToObservableCollection();
                //Invoices = query?.Include(f => f.Client)?.Include(f => f.Employee)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt).ToObservableCollection();
                if (!string.IsNullOrEmpty(InvoiceNameSearch?.ToString()))
                {
                    Invoices = Invoices.Where(f => f.Name.ToLower().Contains(InvoiceNameSearch?.ToString().ToLower())).OrderByDescending(f => f.DateTimestamp).ToObservableCollection();
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
                foreach (var i in Clients) ImgLoading(i);
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

                // Устанавливаем Планы лечения для вкладки "Врачебная" и зубную карту
                SetClientStages(model);
                SetTeeth(model);

                // загружаем встречи для вкладки "Посещения"
                Appointments = db.Appointments.Where(f => f.ClientInfoId == model.Id).Include(f => f.Employee).Include(f => f.Service).OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Appointments>();

                // вкладка "Документы"
                Document = new ClientsDocumentsViewModel();

                FieldsViewModel = new FieldsViewModel(model, db);
                EventChangeReadOnly += FieldsViewModel.ChangedReadOnly;

                FieldsViewModel.IsReadOnly = true;

                SetTabVisibility(FieldsViewModel.AdditionalFieldsVisible);
                PathToUserFiles = Path.Combine(Config.PathToFilesDirectory, Model?.Guid);
                Files = Directory.Exists(PathToUserFiles) ? new DirectoryInfo(PathToUserFiles).GetFiles().ToObservableCollection() : new ObservableCollection<FileInfo>();
                LoadDocuments();
            }
            catch (Exception e)
            {

            }
        }
        #endregion


        #region Работа с разделом карты "Счета"
        #region Счета
        [Command]
        public void OpenFormInvoice(object p)
        {
            try
            {
                Invoice invoice;

                if (p is Invoice inv) invoice = inv;
                else
                {
                    var date = DateTime.Now;
                    invoice = new Invoice()
                    {
                        Number = NewInvoiceNumberGenerate(),
                        Date = date.ToString(),
                        DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                        Paid = 0
                    };
                }

                var advertisings = db.Advertising.ToArray();
                var vm = new InvoiceVM(Employees, advertisings)
                {
                    Name = invoice.Name,
                    Number = invoice.Number,
                    Date = invoice.Date,
                    DateTimestamp = invoice.DateTimestamp,
                    Employee = Employees.FirstOrDefault(f => f.Id == invoice?.Employee?.Id),
                    Advertising = advertisings.FirstOrDefault(f => f.Id == invoice.AdvertisingId),
                    Paid = invoice.Paid,
                    Model = invoice
                };
                vm.EventSave += SaveInvoice;
                new InvoiceWindow() { DataContext = vm }.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму счета!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public void SaveInvoice(object p)
        {
            try
            {
                if (p is Invoice invoice)
                {
                    invoice.Client = Model;
                    invoice.ClientId = Model.Id;

                    if (invoice.Id == 0)
                    {
                        db.Entry(invoice).State = EntityState.Added;
                        ClientInvoices?.Add(invoice);
                        Invoices?.Add(invoice);
                    }

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

                    if (db.SaveChanges() > 0) new Notification() { Content = "Счет сохранен в базу данных!" }.run();
                }
            }
            catch
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
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить счет?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    db.InvoiceItems.Where(f => f.InvoiceId == invoice.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.Entry(invoice).State = EntityState.Deleted;

                    // может не оказаться этого эл-та в списке, например, он в другом статусе
                    var inv = Invoices.FirstOrDefault(f => f.Id == invoice.Id);
                    if (inv != null) Invoices.Remove(inv);

                    var clientInv = ClientInvoices.FirstOrDefault(f => f.Id == invoice.Id);
                    if (clientInv != null) ClientInvoices.Remove(clientInv);

                    if (db.SaveChanges() > 0) new Notification() { Content = "Счет удален из базы данных!" }.run();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить счет из базы данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private string NewInvoiceNumberGenerate() => int.TryParse(db.Invoices?.ToList()?.OrderByDescending(f => f.Id)?.FirstOrDefault()?.Number, out int result) ? string.Format("{0:00000000}", ++result) : "00000001";

        #endregion

        #region Позиция в смете

        [Command]
        public void OpenFormInvoiceItem(object p)
        {
            try
            {
                if (p is InvoiceItemCommandParameters parameter)
                {
                    var vm = new InvoiceItemVM(parameter.Param, db)
                    {
                        Invoice = parameter.Invoice,
                        Type = parameter.Param,
                        Title = parameter.Param == 0 ? "Услуга" : "Номенклатура",
                        Model = new InvoiceItems() { Invoice = parameter.Invoice, InvoiceId = parameter.Invoice?.Id },
                    };
                    vm.EventSave += SaveInvoiceItem;
                    new InvoiceItemWindow() { DataContext = vm }.Show();
                }

                if (p is InvoiceItems item)
                {
                    IInvoiceItem selected = null;
                    if (item.Type == 0) selected = db.Services.FirstOrDefault(f => f.Id == item.ItemId);
                    else selected = db.Nomenclature.FirstOrDefault(f => f.Id == item.ItemId);

                    var vm = new InvoiceItemVM(item.Type, db)
                    {
                        Invoice = item.Invoice,
                        Type = item.Type,
                        Count = item.Count,
                        Price = item.Price,
                        Title = item.Type == 0 ? "Услуга" : "Номенклатура",
                        SelectedItem = selected,
                        Element = selected,
                        Model = item
                    };
                    vm.EventSave += SaveInvoiceItem;
                    new InvoiceItemWindow() { DataContext = vm }.Show();
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму добавления услуги!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void SaveInvoiceItem(object p)
        {
            try
            {
                if (p is InvoiceItems item)
                {
                    if (item.Id == 0) { db.Entry(item).State = EntityState.Added; }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void DeleteInvoiceItem(object p)
        {
            try
            {
                if (p is InvoiceItems item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить услугу в счете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    item.Invoice = null;

                    db.InvoiceItems.Remove(item);
                    db.SaveChanges();
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
                        string connectionString = new ApplicationContext().Database.Connection.ConnectionString;               
                        var con = "XpoProvider=SQLite;" + connectionString;
                        source.ConnectionParameters = new CustomStringConnectionParameters(con);
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

        #region Работа с разделом карты "Врачебная"
        //Свойства
        public ObservableCollection<TreatmentStage> ClientStages
        {
            get { return GetProperty(() => ClientStages); }
            set { SetProperty(() => ClientStages, value); }
        }
        public PatientTeeth Teeth
        {
            get { return GetProperty(() => Teeth); }
            set { SetProperty(() => Teeth, value); }
        }
        public ChildTeeth ChildTeeth
        {
            get { return GetProperty(() => ChildTeeth); }
            set { SetProperty(() => ChildTeeth, value); }
        }

        public void SetTeeth(Client client)
        {
            try
            {
                if (client?.Id != 0 && client.Teeth?.Length > 100) Teeth = JsonSerializer.Deserialize<PatientTeeth>(client.Teeth);
                else Teeth = new PatientTeeth();

                if (client?.Id != 0 && client.ChildTeeth?.Length > 100) ChildTeeth = JsonSerializer.Deserialize<ChildTeeth>(client.ChildTeeth);
                else ChildTeeth = new ChildTeeth();
            }
            catch
            {
                Teeth = new PatientTeeth();
                ChildTeeth = new ChildTeeth();
            }
        }

        private void SetClientStages(Client client) => ClientStages = client?.Id == 0 ? new ObservableCollection<TreatmentStage>() : db.TreatmentStage.Where(f => f.ClientId == client.Id).Include(f => f.Client).ToObservableCollection();

        [Command]
        public void OpenTreatmentForm(object p)
        {
            try
            {
                if (p is TreatmentStage model)
                {
                    var vm = new TreatmentStageVM() { Date = model?.Date, Name = model?.Name, Model = model };
                    vm.EventSave += SaveTreatment;
                    new TreatmentStageWindow() { DataContext = vm }.Show();
                }
                else
                {
                    var vm = new TreatmentStageVM() { Model = new TreatmentStage() { Client = Model, ClientId = Model.Id } };
                    vm.EventSave += SaveTreatment;
                    new TreatmentStageWindow() { DataContext = vm }?.Show();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void SaveTreatment(object p)
        {
            try
            {
                if (p is TreatmentStage model)
                {
                    if (model?.Date == null) model.Date = DateTime.Now.ToShortDateString();
                    if (string.IsNullOrEmpty(model?.Name)) model.Date = "Без названия";
                    if (model?.Id == 0)
                    {
                        db?.TreatmentStage.Add(model);
                        ClientStages?.Add(model);
                    }
                }
                if (db.SaveChanges() > 0) new Notification() { Content = "Сохранено в базу данных!" }.run();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке добавить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void DeleteTreatment(object p)
        {
            try
            {
                if (p is TreatmentStage model)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить область?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                    if (response.ToString() == "No") return;

                    db.TreatmentStage.Remove(model);
                    if (db.SaveChanges() > 0) new Notification() { Content = "Удалено из базы данных!" }.run();
                    ClientStages.Remove(model);
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удаления области!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenFormTemplate(object p)
        {
            if (p is TreatmentParameters parameters)
            {
                if (parameters.Model is TreatmentStage model)
                {
                    var vm = new SelectTemplateInTreatmentStageVM() { TemplateName = parameters.Name, Model = model, VM = this };

                    switch (parameters.Name)
                    {
                        case "Complaint":
                            vm.Templates = db.Complaints.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Anamnes":
                            vm.Templates = db.Anamneses.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Objectivly":
                            vm.Templates = db.Objectively.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "DescriptionXRay":
                            vm.Templates = db.DescriptionXRay.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Diagnos":
                            vm.Templates = db.Diagnoses.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Plan":
                            vm.Templates = db.TreatmentPlans.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Treatment":
                            vm.Templates = db.Diaries.Select(f => new TreeTemplate() // override on Treatment
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                        case "Allergy":
                            vm.Templates = db.Allergies.Select(f => new TreeTemplate()
                            {
                                Id = f.Id,
                                IsDir = f.IsDir,
                                ParentId = f.ParentId,
                                Name = f.Name
                            }).ToArray(); break;
                    }
                    new SelectValueInTemplateWin() { DataContext = vm }?.ShowDialog();
                }
            }
        }

        [Command]
        public void AddChecked(object p)
        {
            try
            {
                if (p is SelectValueInTemplateWin win && win.view is TreeListView tree && tree.DataContext is SelectTemplateInTreatmentStageVM vm)
                {
                    var values = vm.Templates.Where(f => f.IsChecked == true).ToArray();
                    var str = new StringBuilder();
                    values.ForEach(f => str.Append(f.Name + "\n"));

                    int idx = ClientStages.IndexOf(f => f.Guid == vm.Model?.Guid);
                    if (idx < 0) return;

                    switch (vm.TemplateName)
                    {
                        case "Complaint": ClientStages[idx].Complaints = str.ToString(); break;
                        case "Anamnes": ClientStages[idx].Anamneses = str.ToString(); break;
                        case "Objectivly": ClientStages[idx].Objectively = str.ToString(); break;
                        case "DescriptionXRay": ClientStages[idx].DescriptionXRay = str.ToString(); break;
                        case "Diagnos": ClientStages[idx].Diagnoses = str.ToString(); break;
                        case "Plan": ClientStages[idx].Plans = str.ToString(); break;
                        case "Treatment": ClientStages[idx].Treatments = str.ToString(); break;
                        case "Allergy": ClientStages[idx].Allergies = str.ToString(); break;
                    }
                    win?.Close();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке добавить значение в поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Clear(object p)
        {
            try
            {
                if (p is TreatmentParameters parameters)
                {
                    if (parameters.Model is TreatmentStage model)
                    {
                        var response = ThemedMessageBox.Show(title: "Внимание", text: "Очистить поле?",
                            messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "No") return;

                        switch (parameters.Name)
                        {
                            case "Complaint": model.Complaints = null; break;
                            case "Anamnes": model.Anamneses = null; break;
                            case "Objectivly": model.Objectively = null; break;
                            case "DescriptionXRay": model.DescriptionXRay = null; break;
                            case "Diagnos": model.Diagnoses = null; break;
                            case "Plan": model.Plans = null; break;
                            case "Treatment": model.Treatments = null; break;
                            case "Allergy": model.Allergies = null; break;
                            case "Recomendation": model.Recommendations = null; break;
                        }
                    }
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке очистить поле!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void ToothMarked(object p)
        {
            try
            {
                if (p is ToothCommandParameters param)
                {
                    switch (param.Diagnos)
                    {
                        case "Healthy": param.Tooth.ToothImagePath = TeethImages.ImgPathGreen; param.Tooth.Abbr = "З"; break;
                        case "Missing": param.Tooth.ToothImagePath = TeethImages.ImgPathGray; param.Tooth.Abbr = "О"; break;
                        case "Impacted": param.Tooth.ToothImagePath = TeethImages.ImgPathGray; param.Tooth.Abbr = "НП"; break;
                        case "Radiks": param.Tooth.ToothImagePath = TeethImages.ImgPathGray; param.Tooth.Abbr = "КН"; break;
                        case "Caries": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "К"; break;
                        case "Pulpit": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "П"; break;
                        case "Gangrene": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "Г"; break;
                        case "Granuloma": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "Гр"; break;
                        case "Deletable": param.Tooth.ToothImagePath = TeethImages.ImgPathRed; param.Tooth.Abbr = "Э"; break;
                        case "MetalCrown": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "КМ"; break;
                        case "Bridge": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "М"; break;
                        case "Rp": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "ПР"; break;
                        case "Seal": param.Tooth.ToothImagePath = TeethImages.ImgPathYellow; param.Tooth.Abbr = "ПЛ"; break;
                        case "Imp": param.Tooth.ToothImagePath = TeethImages.ImgPathImp; param.Tooth.Abbr = "Имп"; break;
                    }
                }
            }
            catch
            {

            }
        }

        public void SaveTeeth(Client client)
        {
            try
            {
                client.Teeth = JsonSerializer.Serialize(Teeth);
                client.ChildTeeth = JsonSerializer.Serialize(ChildTeeth);
            }
            catch
            {
                client.Teeth = null;
                client.ChildTeeth = null;
            }
        }
        #endregion

        #region Работа с разделом карты "Дополнительные поля"
        public ObservableCollection<Appointments> Appointments
        {
            get { return GetProperty(() => Appointments); }
            set { SetProperty(() => Appointments, value); }
        }
        #endregion

        #region Документы
        [Command]
        public void OpenFormDocuments()
        {
            try
            {
                Window wnd = Application.Current.Windows.OfType<Window>().Where(w => w.ToString() == DocumentsWindow?.ToString()).FirstOrDefault();
                if (wnd != null)
                {
                    wnd.Activate();
                    return;
                }

                var vm = new ClientsDocumentsViewModel();
                vm.EventUpdateDocuments += LoadDocuments;
                DocumentsWindow = new DocumentsWindow() { DataContext = vm };
                DocumentsWindow.Show();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public void LoadDocuments()
        {
            try
            {
                Documents = new ObservableCollection<FileInfo>();
                if (Directory.Exists(Config.PathToClientsDocumentsDirectory))
                {
                    IEnumerable<string> filesNames = new List<string>();
                    string[] formats = new string[] { "*.docx", "*.doc", "*.rtf", "*.odt", "*.epub", "*.txt", "*.html", "*.htm", "*.mht", "*.xml" };
                    foreach (var format in formats)
                    {
                        var collection = Directory.EnumerateFiles(Config.PathToClientsDocumentsDirectory, format).ToList();
                        if (collection.Count > 0) filesNames = filesNames.Union(collection);
                    }
                    foreach (var filePath in filesNames) Documents.Add(new FileInfo(filePath));
                }
            }
            catch (Exception e)
            {

            }

        }

        public ObservableCollection<FileInfo> Documents
        {
            get { return GetProperty(() => Documents); }
            set { SetProperty(() => Documents, value); }
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

        public DocumentsWindow DocumentsWindow { get; set; }

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
                    SaveTeeth(Model);
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

                    db.TreatmentStage.Where(f => f.ClientId == Model.Id).ForEach(f => db.Entry(f).State = EntityState.Deleted);
                    db.SaveChanges();
                
                // удаляем фото 
                if (Directory.Exists(Config.PathToClientsPhotoDirectory))
                {
                    var photo = Directory.GetFiles(Config.PathToClientsPhotoDirectory).FirstOrDefault(f => f.Contains(Model?.Guid));
                    if (photo != null && File.Exists(photo)) File.Delete(photo);
                }

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

        public ClientsDocumentsViewModel Document
        {
            get { return GetProperty(() => Document); }
            set { SetProperty(() => Document, value); }
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

        #region Управление фото

        [Command]
        public void ImageSave(object p)
        {
            try
            {
                /*if (p is ImageEditEx param)
                {
                    byte[] imageData;
                    using (FileStream fs = new FileStream(param.ImagePath, FileMode.Open))
                    {
                        imageData = new byte[fs.Length];
                        fs.Read(imageData, 0, imageData.Length);
                        Model.Img = imageData;
                        db.SaveChanges();
                    }
                }*/

                if (p is ImageEditEx param)
                {
                    if (!Directory.Exists(Config.PathToClientsPhotoDirectory)) Directory.CreateDirectory(Config.PathToClientsPhotoDirectory);

                    var oldPhoto = Directory.GetFiles(Config.PathToClientsPhotoDirectory).FirstOrDefault(f => f.Contains(param?.ImageGuid));

                    if (oldPhoto != null && File.Exists(oldPhoto))
                    {
                        var response = ThemedMessageBox.Show(title: "Вы уверены?", text: "Заменить текущее фото клиента?",
                        messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                        if (response.ToString() == "No") return;
                        File.SetAttributes(oldPhoto, FileAttributes.Normal);
                        File.Delete(oldPhoto);
                    }

                    FileInfo photo = new FileInfo(Path.Combine(param.ImagePath));
                    string fileFullName = Path.Combine(Config.PathToClientsPhotoDirectory, param.ImageGuid + photo.Extension);
                    photo.CopyTo(fileFullName, true);
                    File.SetAttributes(fileFullName, FileAttributes.Normal);
                    new Notification() { Content = "Фото клиента сохраненo!" }.run();
                    var model = Clients.FirstOrDefault(f => f.Guid == param?.ImageGuid);
                    if (model != null)
                    {
                        model.Photo = param.ImagePath;
                        if (param.EditValue is ImageSource img) model.Image = img;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void ImageDelete(object p)
        {
            try
            {
                if (p is ImageEditEx img)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание", text: "Удалить фото клиента?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                    if (response.ToString() == "No") return;

                    var file = Directory.GetFiles(Config.PathToClientsPhotoDirectory).FirstOrDefault(f => f.Contains(img?.ImageGuid));

                    if (file != null) 
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file); 
                    }
                    img?.Clear();
                    var model = Clients.FirstOrDefault(f => f.Guid == img?.ImageGuid);
                    if (model != null)
                    {
                        model.Photo = null;
                        model.Image = null;
                    }
                    new Notification() { Content = "Фото клиента удалено!" }.run();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

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