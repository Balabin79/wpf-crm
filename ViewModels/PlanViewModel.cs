using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Logs;
using Dental.Models;
using Dental.Reports;
using Dental.Services;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using License;
using System.Threading.Tasks;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.ViewModels.ClientDir;

namespace Dental.ViewModels
{
    class PlanViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        public PlanViewModel(ClientsViewModel vm)
        {
            try
            {
                db = new ApplicationContext();

                Plans = db.Plans?.Include(f => f.Employee).Include(f => f.Client).Include(f => f.PlanStatus).OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Plan>();

                ClientCategories = vm.ClientCategories;
                Prices = vm.Prices;
                Advertisings = vm.Advertisings;
                Employees = vm.Employees;
                Client = vm.Model;

                PlanStatuses = db.PlanStatuses.OrderBy(f => f.Sort).ToArray();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка подключения к базе данных при попытке загрузить список планов клиентов!",
                         messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICollection<ClientCategory> ClientCategories { get; private set; }
        public ICollection<Employee> Employees { get; private set; }
        public ICollection<Advertising> Advertisings { get; private set; }
        public ICollection<PlanStatus> PlanStatuses { get; private set; }
        public ICollection<Plan> Plans { get; private set; }
        public Client Client { get; private set; }

        #region Планы работ
        [Command]
        public void Add(object p)
        {
            try
            {
                if (p is Client client)
                {
                    var date = DateTime.Now;
                    Plans.Add(new Plan
                    {
                        Date = date.ToString(),
                        DateTimestamp = new DateTimeOffset(date).ToUnixTimeSeconds(),
                        Client = client,
                        ClientId = client?.Id,
                        PlanStatusId = 5
                    });
                }
            }
            catch (Exception e)
            {

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
                if (!Status.Licensed && (Status.Evaluation_Time_Current > Status.Evaluation_Time))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Пробный период истек! Вам необходимо приобрести лицензию.",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                #endregion

                foreach (var plan in Plans.ToList())
                {
                    if (plan.Id == 0)
                    {
                        db.Entry(plan).State = EntityState.Added;
                        Plans?.Add(plan);
                    }

                    if (plan.PlanItems?.Count > 0)
                    {
                        var items = plan.PlanItems;

                        foreach (var i in items.ToList())
                        {
                            if (string.IsNullOrEmpty(i.Name))
                            {
                                i.Plan = null;
                                db.Entry(i).State = EntityState.Detached;
                                plan.PlanItems.Remove(i);
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
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке сохранить план в базе данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void Delete(object p)
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
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Ошибка при попытке удалить план из базы данных!", messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        #endregion

        #region Позиция в плане
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
                        if (((GridCellData)parameters.Popup.DataContext).Row is PlanItem item)
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
        public void AddItem(object p)
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

            }
        }

        [Command]
        public void DeleteItem(object p)
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
                (new ViewModelLog(e)).run();
            }
        }
        #endregion

        #region Печать плана
        [Command]
        public void Print(object p)
        {
           /* try
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
            }*/
        }
        #endregion
    }
}
