using B6CRM.Infrastructures.Converters;
using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Reports;
using B6CRM.Services;
using B6CRM.Views.WindowForms;
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


namespace B6CRM.ViewModels.ClientDir
{
    public class ClientPlansViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public ClientPlansViewModel(Client client)
        {
            db = new ApplicationContext();
            Config = db.Config;
            Prices = db.Services.Where(f => f.IsHidden != 1)?.OrderBy(f => f.Sort).ToArray();

            Client = client?.Id > 0 ? db.Clients.FirstOrDefault(f => f.Id == client.Id) : new Client();
            Load();
        }

        #region Права на выполнение команд
        public bool CanAddPlan(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanSavePlan() => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanDeletePlan(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanDelitable;

        public bool CanAddPlanItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanDeletePlanItem(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanMovedToInvoice(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PlanEditable;
        public bool CanPrintPlan(object p) => ((UserSession)Application.Current.Resources["UserSession"]).PrintPlan;
        #endregion

        [Command]
        public void Load()
        {

            PlanStatuses = db.PlanStatuses.OrderBy(f => f.Sort).ToArray();

            Plans = db.Plans?.Where(f => f.ClientId == Client.Id)?.
                Include(f => f.Client).
                Include(f => f.PlanStatus).
                Include(f => f.PlanItems).
                OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Plan>();

        }

        #region Планы работ
        [Command]
        public void AddPlan(object p)
        {
            try
            {
                if (Client == null) return;
                var date = DateTime.Now;
                PlanStatus status = db.PlanStatuses.FirstOrDefault(s => s.Id == 5);

                var model = new Plan
                {
                    Date = date.ToString(),
                    DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                    Client = Client,
                    ClientId = Client?.Id,
                    PlanStatus = status,
                    PlanStatusId = status?.Id
                };

                db.Add(model);
                Plans.Add(model);

                if (db.SaveChanges() > 0) new Notification() { Content = "Добавлен новый план в базу данных!" }.run();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        [Command]
        public void Save()
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
                Log.ErrorHandler(e, "Ошибка при попытке сохранить план в базу данных!", true);
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
                /*  if (p is Plan plan)
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
                  }*/
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


        public ObservableCollection<Plan> Plans
        {
            get { return GetProperty(() => Plans); }
            set { SetProperty(() => Plans, value); }
        }

        public ICollection<PlanStatus> PlanStatuses { get; private set; }

        private Client Client { get; set; }

        public Config Config
        {
            get { return GetProperty(() => Config); }
            set { SetProperty(() => Config, value); }
        }

        public object[] Prices { get; set; }

        public object SelectedItem
        {
            get { return GetProperty(() => SelectedItem); }
            set { SetProperty(() => SelectedItem, value); }
        }
    }
}
