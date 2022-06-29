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
using System.Collections;
using DevExpress.Xpf.Bars;
using DevExpress.Utils.Svg;
//using DevExpress.XtraPrinting.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Dental.Infrastructures.Commands;
using Dental.Views.Documents;
using Dental.Services.Files;
using System.IO;
using DevExpress.Xpf.Grid;

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
                Client = client ?? new Client();
                SetInvoices();
                PrintMenuLoading();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Счета\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

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
                if (p != null) Invoice = Invoices.FirstOrDefault(f => f.Id == (int)p);                
                else
                {
                    title = "Добавить счет";
                    Invoice = new Invoice()
                    {
                        Number = NewNumberGenerate(),
                        Client = Client,
                        ClientId = Client.Id,
                        Date = DateTime.Now.ToString(),
                        Paid = 0
                    };
                }

                InvoiceVM = new InvoiceVM(db)
                {
                    Title = title,
                    Number = Invoice.Number,
                    Date = Invoice.Date,
                    Client = Invoice.Client,
                    Paid = Invoice.Paid,
                    Total = Invoice.Total,
                    ClientFieldVisibility = visibility
                };
                var height = fromPatientCard ? 230 : 270;
                InvoiceWindow = new InvoiceWindow() { DataContext = this, Height = height, MaxHeight = height };
                InvoiceWindow.ShowDialog();
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму счета!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void SaveInvoice()
        {
            try
            {               
                if (InvoiceVM.Client == null) return;
                bool edited = Invoice.Id != 0 && (Invoice.Client != InvoiceVM.Client || Invoice.Date != InvoiceVM.Date);

                Invoice.Number = InvoiceVM.Number;
                Invoice.Date = InvoiceVM.Date;
                Invoice.Client = InvoiceVM.Client;
                Invoice.Paid = InvoiceVM.Paid;
                Invoice.Total = InvoiceVM.Total;

                if (Invoice.Id == 0)
                {
                    db.Entry(Invoice).State = EntityState.Added;
                    Invoices.Add(Invoice);
                }
                db.SaveChanges();
                if (edited)
                {
                     Invoices.Remove(Invoices.FirstOrDefault(f => f.Id == Invoice.Id));
                     Invoices.Add(Invoice);
                }
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

        [Command]
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
                    Invoices.Remove(invoice);
                    db.SaveChanges();
                }
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить счет из базы данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

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

        #region Услуги в смете
        [Command]
        public void SelectItemInServiceField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Service service)
                    {
                        if (service.IsDir == 1) return;
                        parameters.Popup.EditValue = service;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch
            {
                
            }
        }

        [Command]
        public void OpenFormInvoiceService(object p)
        {
            try
            {
                Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
                Services = db.Services.ToArray();
                if (p is Invoice invoice)
                {
                    InvoiceServiceItem = new InvoiceServiceItems();
                    InvoiceServiceItemVM = new InvoiceServiceItemVM() { Invoice = invoice, Title = "Добавление услуги" };
                    InvoiceServiceWindow = new InvoiceServiceWindow() { DataContext = this };
                    InvoiceServiceWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке открыть форму добавления услуги!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void EditInvoiceService(object p)
        {
            try
            {
                if (p is InvoiceServiceItems item)
                {
                    Employees = db.Employes.OrderBy(f => f.LastName).ToArray();
                    Services = db.Services.ToArray();
                    InvoiceServiceItemVM = new InvoiceServiceItemVM()
                    {
                        Invoice = item.Invoice,
                        Employee = item.Employee,
                        Service = item.Service,
                        Count = item.Count,
                        Price = item.Price,
                        Title = "Редактирование услуги"
                    };
                    InvoiceServiceItem = item;
                    InvoiceServiceWindow = new InvoiceServiceWindow() { DataContext = this };
                    InvoiceServiceWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveRowInInvoice()
        {
            if (string.IsNullOrEmpty(InvoiceServiceItemVM.Service?.Name)) return;
            try
            {
                InvoiceServiceItem.Invoice = InvoiceServiceItemVM.Invoice;
                InvoiceServiceItem.Service = InvoiceServiceItemVM.Service;
                InvoiceServiceItem.Employee = InvoiceServiceItemVM.Employee;
                InvoiceServiceItem.Count = InvoiceServiceItemVM.Count;
                InvoiceServiceItem.Price = InvoiceServiceItemVM.Price;
                var invoice = Invoices.FirstOrDefault(f => f.Id == InvoiceServiceItem.Invoice.Id);
                if (InvoiceServiceItem.Id == 0)
                {

                    //db.Entry(InvoiceServiceItem).State = EntityState.Added;
                    invoice.InvoiceServiceItems.Add(InvoiceServiceItem);

                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
            finally
            {
                InvoiceServiceWindow?.Close();
            }
        }

        [Command]
        public void DeleteInvoiceService(object p)
        {
            try
            {
                if (p is InvoiceServiceItems item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить услугу в счете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void CancelFormInvoiceItem(object p)
        {
            try
            {
                if (p is System.ComponentModel.CancelEventArgs arg)
                {
                    arg.Cancel = false;
                    return;
                }
                InvoiceServiceWindow?.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Материалы в смете
        [Command]
        public void SelectItemInMaterialField(object p)
        {
            try
            {
                if (p is FindCommandParameters parameters)
                {
                    if (parameters.Tree.FocusedRow is Nomenclature material)
                    {
                        if (material.IsDir == 1) return;
                        parameters.Popup.EditValue = material;
                    }
                    parameters.Popup.ClosePopup();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void OpenFormInvoiceMaterial(object p)
        {
            try
            {
                Materials = db.Nomenclature.ToArray();

                if (p is Invoice invoice)
                {
                    InvoiceMaterialItem = new InvoiceMaterialItems();
                    InvoiceMaterialItemVM = new InvoiceMaterialItemVM() { Invoice = invoice, Title = "Добавление материала" };
                    InvoiceMaterialWindow = new InvoiceMaterialWindow() { DataContext = this };
                    InvoiceMaterialWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void EditInvoiceMaterial(object p)
        {
            try
            {
                if (p is InvoiceMaterialItems item)
                {
                    Materials = db.Nomenclature.ToArray();
                    Measuries = db.Measure.OrderBy(f => f.Name).ToArray();
                    InvoiceMaterialItemVM = new InvoiceMaterialItemVM()
                    {
                        Invoice = item.Invoice,
                        Nomenclature = item.Nomenclature,
                        Count = item.Count,
                        Price = item.Price,
                        Title = "Редактирование материала"
                    };
                    InvoiceMaterialItem = item;
                    InvoiceMaterialWindow = new InvoiceMaterialWindow() { DataContext = this };
                    InvoiceMaterialWindow.ShowDialog();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveMaterialRowInInvoice()
        {
            if (string.IsNullOrEmpty(InvoiceMaterialItemVM.Nomenclature?.Name)) return;
            try
            {
                InvoiceMaterialItem.Invoice = InvoiceMaterialItemVM.Invoice;
                InvoiceMaterialItem.Nomenclature = InvoiceMaterialItemVM.Nomenclature;
                InvoiceMaterialItem.Count = InvoiceMaterialItemVM.Count;
                InvoiceMaterialItem.Price = InvoiceMaterialItemVM.Price;

                if (InvoiceMaterialItem.Id == 0)
                {
                    db.Entry(InvoiceMaterialItem).State = EntityState.Added;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
            finally
            {
                InvoiceMaterialWindow?.Close();
            }
        }

        [Command]
        public void DeleteInvoiceMaterial(object p)
        {
            try
            {
                if (p is InvoiceMaterialItems item)
                {
                    var response = ThemedMessageBox.Show(title: "Внимание!", text: "Удалить материал в счете?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                    if (response.ToString() == "No") return;

                    db.Entry(item).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void CancelFormInvoiceMaterialItem(object p)
        {
            try
            {
                if (p is System.ComponentModel.CancelEventArgs arg)
                {
                    arg.Cancel = false;
                    return;
                }
                InvoiceMaterialWindow?.Close();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Печать
        [Command]
        public void OpenFormDocuments()
        {
            try
            {
                Documents.PrintMenuUpdating += PrintMenuLoading;
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
            ?  string.Format("{0:00000000}", ++result) : "00000001";


        public ObservableCollection<Invoice> Invoices
        {
            get { return GetProperty(() => Invoices); }
            set { SetProperty(() => Invoices, value); }
        }

        private void SetInvoices(int? showPaid = null)
        {
            var query = (fromPatientCard && Client.Id > 0) ? db.Invoices.Where(f => f.ClientId == Client.Id) : db.Invoices.Include(f => f.Client);
            if (showPaid != null) query = query.Where(f => f.Paid == showPaid);
            Invoices =   query.Include(f => f.InvoiceServiceItems.Select(x => x.Employee))
                        .Include(f => f.InvoiceServiceItems.Select(x => x.Service))
                        .Include(f => f.InvoiceMaterialItems.Select(x => x.Nomenclature.Measure))
                        .ToObservableCollection() ?? new ObservableCollection<Invoice>();
    }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Measure> Measuries { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Nomenclature> Materials { get; set; }
        public InvoiceVM InvoiceVM { get; set; }
        public InvoiceServiceItemVM InvoiceServiceItemVM { get; set; }
        public InvoiceMaterialItemVM InvoiceMaterialItemVM { get; set; }
        public Invoice Invoice { get; set; }
        public InvoiceServiceItems InvoiceServiceItem { get; set; }
        public InvoiceMaterialItems InvoiceMaterialItem { get; set; }
        public Client Client { get; set; }
        private InvoiceWindow InvoiceWindow;
        public InvoiceServiceWindow InvoiceServiceWindow;
        public InvoiceMaterialWindow InvoiceMaterialWindow;

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
    }
}

