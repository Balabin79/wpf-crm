using B6CRM.Models;
using B6CRM.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.ClientDir
{
    internal class PlansListViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public PlansListViewModel()
        {
            try
            {
                db = new ApplicationContext();
                LoadPlans();
                PlanStatuses = db.PlanStatuses.ToArray();
                LoadClients();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка подключения к базе данных при попытке загрузить список планов работ!", true);
            }
        }

        public void LoadPlans()
        {
            // общие планы
            Plans = db.Plans?.
                Include(f => f.Client).
                OrderByDescending(f => f.CreatedAt).ToObservableCollection() ?? new ObservableCollection<Plan>();
        }

        public void LoadClients(int? isArhive = 0) =>
            Clients = db.Clients.Where(f => f.IsInArchive == isArhive).OrderBy(f => f.LastName).ToObservableCollection() ?? new ObservableCollection<Client>();

        #region Работа с фильтрами и поиском в списке инвойсов
        public object ClientSearch { get; set; }
        public object DateFromSearch { get; set; }
        public object DateToSearch { get; set; }
        public object PlanNameSearch { get; set; }
        public object PlanStatusSearch { get; set; }

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
                if (int.TryParse(PlanStatusSearch?.ToString(), out int statusId) && statusId != 0) where.Add("PlanStatusId=" + statusId.ToString());

                if (DateFromSearch != null && DateTime.TryParse(DateFromSearch?.ToString(), out DateTime dateTimeFrom))
                {
                    dateFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
                }

                if (DateToSearch != null && DateTime.TryParse(DateToSearch?.ToString(), out DateTime dateTimeTo))
                {
                    dateTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();
                }

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

                Plans = db.Plans.FromSqlRaw("SELECT * FROM Plans " + parameters + " ORDER BY DateTimestamp DESC").ToObservableCollection();

                if (!string.IsNullOrEmpty(PlanNameSearch?.ToString()))
                {
                    Plans = Plans.Where(f => f.Name.Contains(PlanNameSearch?.ToString().ToLower())).OrderByDescending(f => f.DateTimestamp).ToObservableCollection();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }
        #endregion

        public ObservableCollection<Plan> Plans
        {
            get { return GetProperty(() => Plans); }
            set { SetProperty(() => Plans, value); }
        }

        public ICollection<PlanStatus> PlanStatuses
        {
            get { return GetProperty(() => PlanStatuses); }
            set { SetProperty(() => PlanStatuses, value); }
        }

        public ObservableCollection<Client> Clients
        {
            get { return GetProperty(() => Clients); }
            set { SetProperty(() => Clients, value); }
        }
    }
}
