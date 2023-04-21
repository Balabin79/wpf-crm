﻿using B6CRM.Infrastructures.Converters;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Reports;
using B6CRM.Services;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using License;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.Parameters;
using System.Diagnostics;
using Telegram.Bot.Types.Payments;
using Invoice = B6CRM.Models.Invoice;

namespace B6CRM.ViewModels.ClientDir
{
    public class ClientInvoicesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ClientInvoicesViewModel()
        {
            db = new ApplicationContext();
            Config = db.Config;
            Prices = db.Services.Where(f => f.IsHidden != 1)?.OrderBy(f => f.Sort).ToArray();
            Advertisings = db.Advertising.ToObservableCollection();
        }

        #region Права на выполнение команд
        public bool CanAddInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanDeleteInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceDelitable;

        public bool CanAddInvoiceItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanDeleteInvoiceItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        public bool CanPrintInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PrintInvoice;
        public bool CanSaveInvoice() => ((UserSession)Application.Current.Resources["UserSession"]).InvoiceEditable;
        #endregion

        [Command]
        public void Load(object p)
        {
            if (p is Client client)
            {
                Client = db.Clients.FirstOrDefault(f => f.Id == client.Id);

                ClientInvoices = client?.Id != 0 ? db.Invoices?.
                    Where(f => f.ClientId == client.Id)?.
                    Include(f => f.Employee)?.
                    Include(f => f.Client)?.
                    Include(f => f.InvoiceItems)?.
                    OrderByDescending(f => f.CreatedAt)?.
                    ToObservableCollection() 
                    : 
                    new ObservableCollection<Invoice>();

                // сбрасываем фильтр счетов в вкарте клиента на значение по умолчание
                ShowPaid = null;
            }
        }

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
                        Load(Client);
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
                    //var inv = Invoices.FirstOrDefault(f => f.Guid == invoice.Guid);
                    //if (inv != null) Invoices.Remove(inv);

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
            var numInvoices = db.Invoices?.ToList()?.OrderByDescending(f => f.Number)?.FirstOrDefault()?.Number;
            var numClientInvoices = ClientInvoices.LastOrDefault()?.Number;

            if (int.TryParse(numInvoices, out int invoicesNumber) && int.TryParse(numClientInvoices, out int clientInvoicesNumber))
            {
                if (clientInvoicesNumber > invoicesNumber) return string.Format("{0:00000000}", ++clientInvoicesNumber);
                return string.Format("{0:00000000}", ++invoicesNumber);
            }

            //есть счета, но нет у пользователя счетов
            if (numInvoices != null && numClientInvoices == null && int.TryParse(numInvoices, out int num)) return string.Format("{0:00000000}", ++num);

            // вообще нет счетов
            return "00000001";
        }
        #endregion

        #region Позиция в смете

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
                        ClientInvoices = db.Invoices?.Where(f => f.ClientId == Client.Id)?.Include(f => f.Employee)?.Include(f => f.Client)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt)?.ToObservableCollection();
                        return;
                    }

                    ClientInvoices = db.Invoices?.Where(f => f.ClientId == Client.Id && f.Paid == ShowPaid)?.Include(f => f.Employee)?.Include(f => f.Client)?.Include(f => f.InvoiceItems)?.OrderByDescending(f => f.CreatedAt)?.ToObservableCollection();
                    return;
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

        /********************************************************************/

        public ObservableCollection<Invoice> ClientInvoices
        {
            get { return GetProperty(() => ClientInvoices); }
            set { SetProperty(() => ClientInvoices, value); }
        }

        public int? ShowPaid
        {
            get { return GetProperty(() => ShowPaid); }
            set { SetProperty(() => ShowPaid, value); }
        }

        private Client Client { get; set; }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

        public ObservableCollection<Advertising> Advertisings
        {
            get { return GetProperty(() => Advertisings); }
            set { SetProperty(() => Advertisings, value); }
        }

        public object[] Prices { get; set; }
    }
}