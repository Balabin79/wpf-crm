using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Views.Invoices;

using DevExpress.Xpf.Bars;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Dental.Infrastructures.Commands;
using Dental.Views.Documents;
using Dental.Services.Files;
using Dental.Services;
using Dental.Models.Base;
using DevExpress.Xpf.Printing;
using Dental.Reports;
using DevExpress.XtraReports.Parameters;
using Dental.Models.Templates;
//using Dental.Reports;

namespace Dental.ViewModels.Invoices
{
    public class InvoicesViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        private readonly bool fromPatientCard;
        public InvoicesViewModel(Client client = null, ApplicationContext context = null, bool fromPatientCard = false)
        {
            try
            {
                this.fromPatientCard = fromPatientCard;
                db = context ?? new ApplicationContext();
                Client = client;
                SetInvoices();
                //PrintMenuLoading();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Счета\"!",
                          messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public bool CanSaveInvoice() => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanOpenFormInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanDeleteInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceDeletable;
        public bool CanCancelFormInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanSelectItemInServiceField(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanOpenFormInvoiceService(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanEditInvoiceService(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanSaveRowInInvoice() => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanDeleteInvoiceService(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanCancelFormInvoiceItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanOpenFormDocuments() => true;
        public bool CanStatusChanged(object p) => true;


        public void StatusReadOnly(bool status)
        {
            IsReadOnly = status;
        }
        public bool IsReadOnly
        {
            get { return GetProperty(() => IsReadOnly); }
            set { SetProperty(() => IsReadOnly, value); }
        }

        #region Счета
        [Command]
        public void OpenFormInvoice(object p)
        {
            try
            {
                Visibility visibility = fromPatientCard == true ? Visibility.Collapsed : Visibility.Visible;
                string title = "Редактирование счета";
                Employees = db.Employes.OrderBy(f => f.LastName).ToArray();

                if (p is Invoice inv) Invoice = inv;
                if (int.TryParse(p.ToString(), out int result)) Invoice = Invoices.FirstOrDefault(f => f.Id == (int)p);
                else
                {
                    title = "Добавить счет";
                    Invoice = new Invoice()
                    {
                        Number = NewNumberGenerate(),
                        Client = Client,
                        ClientId = Client?.Id,
                        Date = DateTime.Now.ToString(),
                        Paid = 0
                    };
                }

                InvoiceVM = new InvoiceVM()
                {
                    Title = title,
                    Number = Invoice.Number,
                    Date = Invoice.Date,
                    Client = Invoice?.Client ?? Client,
                    Employee = Invoice?.Employee,
                    Paid = Invoice.Paid,
                    Total = Invoice.Total,
                    ClientFieldVisibility = visibility,
                    Clients = db.Clients.OrderBy(f => f.LastName).ToArray()
                };
                var height = fromPatientCard ? 275 : 320;
                InvoiceWindow = new InvoiceWindow() { DataContext = this, Height = height, MaxHeight = height };
                InvoiceWindow.ShowDialog();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму счета!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void SaveInvoice()
        {
            try
            {
                var client = InvoiceVM.Client ?? Client;
                if (client == null) return;
                bool edited = Invoice.Id != 0 && (Invoice.Client != client || Invoice.Date != InvoiceVM.Date || Invoice.Paid != InvoiceVM.Paid || Invoice.Employee != InvoiceVM.Employee);

                Invoice.Number = InvoiceVM.Number;
                Invoice.Date = InvoiceVM.Date;
                Invoice.Client = client;
                Invoice.Employee = InvoiceVM?.Employee;
                Invoice.EmployeeId = InvoiceVM?.Employee?.Id;
                Invoice.Paid = InvoiceVM.Paid;
                Invoice.Total = InvoiceVM.Total;

                if (Invoice.Id == 0)
                {
                    db.Entry(Invoice).State = EntityState.Added;
                    // если статус счета(оплачен или нет)  не отличается от статуса фильтра или статус фильтра "Показывать все",  то тогда добавить
                    if (ShowPaid == Invoice?.Paid || ShowPaid == null) Invoices?.Add(Invoice);
                }

                if (edited)
                {
                    if (ShowPaid == Invoice.Paid || ShowPaid == null)
                    {
                        var invoice = Invoices.FirstOrDefault(f => f.Id == Invoice.Id);
                        if (invoice == null) Invoices?.Add(Invoice);
                        else
                        {
                            Invoices?.Remove(invoice);
                            Invoices?.Add(Invoice);
                        }

                    }
                    else // иначе если статусы отличаются, то только удалить из отображаемого списка
                    {
                        var invoice = Invoices.FirstOrDefault(f => f.Id == Invoice.Id);
                        if (invoice != null)
                        {
                            Invoices?.Remove(invoice);
                        }
                    }
                }
                db.SaveChanges();
                Dental.Services.Reestr.Update((int)Tables.Invoices);
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке сохранить счет в базе данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
            finally
            {
                InvoiceWindow?.Close();
            }
        }

    /*    [Command]
        public void DeleteInvoice(object p)
        {
            try
            {
                if (p is Invoice invoice)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить счет?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;
                    db.InvoiceServiceItems.Where(f => f.InvoiceId == invoice.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.InvoiceMaterialItems.Where(f => f.InvoiceId == invoice.Id).ToArray().ForEach(i => db.Entry(i).State = EntityState.Deleted);
                    db.Entry(invoice).State = EntityState.Deleted;

                    // может не оказаться этого эл-та в списке, например, он в другом статусе
                    if (Invoices.Count(f => f.Id == invoice.Id) > 0) Invoices.Remove(invoice);
                    db.SaveChanges();
                    Dental.Services.Reestr.Update((int)Tables.Invoices);
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить счет из базы данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }*/

        [Command]
        public void CancelFormInvoice(object p)
        {
            try
            {
                if (p is System.ComponentModel.CancelEventArgs arg)
                {
                    arg.Cancel = false;
                    return;
                }
                InvoiceWindow?.Close();
            }
            catch
            {

            }
        }
        #endregion

        #region Позиция в смете

        [Command]
        public void OpenFormInvoiceItem(object p)
        {
            try
            {       

                if (p is InvoiceItemCommandParameters parameter)
                {
                    var vm = new InvoiceItemVM(parameter.Param)
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
                    object selected = null;
                    if (item.Type == 0) selected = db.Services.FirstOrDefault(f => f.Id == item.ItemId);
                    else selected = db.Nomenclature.FirstOrDefault(f => f.Id == item.ItemId);

                    var vm = new InvoiceItemVM(item.Type)
                    {
                        Invoice = item.Invoice,
                        Type = item.Type,
                        Count = item.Count,
                        Price = item.Price,
                        Title = item.Type == 0 ? "Услуга" : "Номенклатура",
                        SelectedItem = selected,
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

        public InvoiceItemWindow InvoiceItemWindow;

        [Command]
        public void SaveInvoiceItem(object p)
        {
            try
            {
                if (p is InvoiceItems item)
                {
                    if (item.Id == 0) {  db.Entry(item).State = EntityState.Added; }
                    int cnt = db.SaveChanges();
                    Services.Reestr.Update((int)Tables.InvoiceServiceItems);
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion
        /* 

         [Command]
         public void DeleteInvoiceService(object p)
         {
             try
             {
                 if (p is InvoiceItems item)
                 {
                     var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить услугу в счете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                     if (response.ToString() == "No") return;
                     item.Invoice = null;
                     db.InvoiceServiceItems.Remove(item);
                     db.SaveChanges();
                     Dental.Services.Reestr.Update((int)Tables.InvoiceServiceItems);
                 }
             }
             catch (Exception e)
             {
                 (new ViewModelLog(e)).run();
             }
         }

 */

        #region Печать
        [Command]
        public void OpenFormDocuments()
        {
            try
            {
                //Documents.PrintMenuUpdating += PrintMenuLoading;
                DocumentsWindow = new DocumentsWindow() { DataContext = Documents };
                DocumentsWindow.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "При открытии формы \"Документы\" возникла ошибка!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        private void PrintMenuLoading()
        {
            try
            {
                ItemLinksSource = new ObservableCollection<BarButtonItem>();
                foreach (var doc in Documents.Files)
                {
                    var item = new BarButtonItem()
                    {
                        Content = doc.Name.Length > 120 ? doc.Name.Substring(0, 119) + "..." : doc.Name,
                        Glyph = new BitmapImage(new Uri("pack://application:,,,/DevExpress.Images.v20.1;component/Images/XAF/Action_Printing_Preview.png")),
                        Command = new PrintDocCommand()
                    };

                    //var binding = new Binding() { ElementName = "grid", Path = new PropertyPath("CurrentItem"), Mode = BindingMode.TwoWay };
                    var binding = new MultiBinding()
                    {
                        Converter = new InvoiceMultiBindingConverter()
                    };

                    binding.Bindings.Add(new Binding() { Path = new PropertyPath("Content"), RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.Self } });
                    binding.Bindings.Add(new Binding() { ElementName = "grid", Path = new PropertyPath("CurrentItem"), Mode = BindingMode.TwoWay });

                    item.SetBinding(BarButtonItem.CommandParameterProperty, binding);
                    ItemLinksSource.Add(item);
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке загрузить меню печати счетов!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public DocumentsWindow DocumentsWindow { get; set; }
        public InvoicesDocumentsViewModel Documents { get; private set; } = new InvoicesDocumentsViewModel();
        public ObservableCollection<BarButtonItem> ItemLinksSource
        {
            get { return GetProperty(() => ItemLinksSource); }
            set { SetProperty(() => ItemLinksSource, value); }
        }
        #endregion

        private string NewNumberGenerate() =>
            int.TryParse(db.Invoices?.ToList()?.OrderByDescending(f => f.Id)?.FirstOrDefault()?.Number, out int result)
            ? string.Format("{0:00000000}", ++result) : "00000001";


        public ObservableCollection<Invoice> Invoices
        {
            get { return GetProperty(() => Invoices); }
            set { SetProperty(() => Invoices, value); }
        }

        private void SetInvoices(int? showPaid = null)
        {
            var query = db.Invoices.Where(f => f.ClientId == Client.Id);

            if (showPaid != null) query = query.Where(f => f.Paid == showPaid);
            Invoices = query.Include(f => f.Client).Include(f => f.Employee)
                .Include(f => f.InvoiceItems)
                .ToObservableCollection() ?? new ObservableCollection<Invoice>();
        }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Measure> Measuries { get; set; }
        public InvoiceVM InvoiceVM
        {
            get { return GetProperty(() => InvoiceVM); }
            set { SetProperty(() => InvoiceVM, value); }
        }

        public Invoice Invoice { get; set; }


        public Client Client
        {
            get { return GetProperty(() => Client); }
            set { SetProperty(() => Client, value); }
        }

        private InvoiceWindow InvoiceWindow;

        // фильтр показывать оплаченные/неоплаченные счета
        [Command]
        public void StatusChanged(object p)
        {
            if (int.TryParse(p?.ToString(), out int param))
            {
                ShowPaid = param;
                if (param == -1)
                {
                    ShowPaid = null;
                    SetInvoices();
                }
                SetInvoices(ShowPaid);
                return;
            }
            ShowPaid = null;
            SetInvoices();
        }

        public int? ShowPaid
        {
            get { return GetProperty(() => ShowPaid); }
            set { SetProperty(() => ShowPaid, value); }
        }

        public void NewClientSaved(Client client)
        {
            Client = db.Clients.FirstOrDefault(f => f.Id == client.Id) ?? new Client();
            if (InvoiceVM != null) InvoiceVM.Client = Client;
        }

        [Command]
        public void PrintInvoice(object p)
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
                PrintHelper.ShowPrintPreview(conv.Page, report);
            }
        }
    }
}

