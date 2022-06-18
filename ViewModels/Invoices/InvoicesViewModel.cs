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
                if (fromPatientCard)
                {
                    Invoices = (Client.Id > 0) ?
                        db.Invoices.Where(f => f.ClientId == Client.Id)
                            .Include(f => f.InvoiceServiceItems.Select(x => x.Employee))
                            .Include(f => f.InvoiceServiceItems.Select(x => x.Service))
                            .Include(f => f.InvoiceMaterialItems.Select(x => x.Nomenclature))
                        .ToObservableCollection() : new ObservableCollection<Invoice>();
                }
                else
                {
                    Invoices = db.Invoices
                        .Include(f => f.Client)
                        .Include(f => f.InvoiceServiceItems.Select(x => x.Employee))
                        .Include(f => f.InvoiceServiceItems.Select(x => x.Service))
                        .Include(f => f.InvoiceMaterialItems.Select(x => x.Nomenclature))
                        .ToObservableCollection() ?? new ObservableCollection<Invoice>();
                }

            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Счета\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
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
                    title = "Новый счет";
                    Invoice = new Invoice()
                    {
                        Number = NewNumberGenerate(),
                        Client = Client,
                        ClientId = Client.Id,
                        Date = DateTime.Now.ToShortDateString(),
                        Paid = 0
                    };
                }

                InvoiceVM = new InvoiceVM(db)
                {
                    Number = Invoice.Number,
                    Date = Invoice.Date,
                    Client = Invoice.Client,
                    Paid = Invoice.Paid,
                    Total = Invoice.Total,
                    ClientFieldVisibility = visibility
                };

                InvoiceWindow = new InvoiceWindow() { DataContext = this };
                InvoiceWindow.ShowDialog();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Command]
        public void SaveInvoice()
        {
            try
            {
                if (InvoiceVM.Client == null) return;
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
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
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
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
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
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
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
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
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
                    InvoiceServiceItemVM = new InvoiceServiceItemVM() { Invoice = invoice, Title = "Добавление новой услуги" };
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

                if (InvoiceServiceItem.Id == 0)
                {
                    db.Entry(InvoiceServiceItem).State = EntityState.Added;
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
                    InvoiceMaterialItemVM = new InvoiceMaterialItemVM() { Invoice = invoice, Title = "Добавление нового материала" };
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

        private string NewNumberGenerate() =>
            int.TryParse(db.Invoices?.ToList()?.OrderByDescending(f => f.Id)?.FirstOrDefault()?.Number, out int result)
            ?  string.Format("{0:00000000}", ++result) : "00000001";

        public ICollection<Invoice> Invoices { get; set; }
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
       
    }
}

