using Dental.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels.Statistics
{
    public class AdvertisingSourcesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public AdvertisingSourcesViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Search();
            }
            catch
            {

            }
        }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

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

        public ObservableCollection<object> Data
        {
            get { return GetProperty(() => Data); }
            set { SetProperty(() => Data, value); }
        }


        [Command]
        public void Search()
        {
            try
            {
                long dateFrom = new DateTimeOffset(new DateTime(1970, 1, 1)).ToUnixTimeSeconds();
                long dateTo = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var date = DateTimeOffset.FromUnixTimeSeconds(dateTo).LocalDateTime;

                if (DateFrom != null && DateTime.TryParse(DateFrom?.ToString(), out DateTime dateTimeFrom))
                {
                    dateFrom = new DateTimeOffset(dateTimeFrom).ToUnixTimeSeconds();
                }

                if (DateTo != null && DateTime.TryParse(DateTo?.ToString(), out DateTime dateTimeTo))
                {
                    dateTo = new DateTimeOffset(dateTimeTo).ToUnixTimeSeconds();
                }

                var query = db.Invoices.Include(f => f.InvoiceItems).Include(f => f.Advertising)
                    .Where(f => f.DateTimestamp >= dateFrom)
                    .Where(f => f.DateTimestamp <= dateTo);

                if (int.TryParse(InvoicesSearchMode?.ToString(), out int paimentStatus))
                {
                    query.Where(f => f.Paid == paimentStatus);
                }

                var invoices = query.ToArray();
                Adv = invoices?.Select(f => f.Advertising).ToArray();

                Stat2D = Adv?.GroupBy(f => f?.Name)?.
                    Select(t => new StatByAdv
                    {
                        // Id = i.Key,
                        Cnt = t.Count(),
                        Name = t.Select(f => f?.Name)?.FirstOrDefault()
                    })?.ToList();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// //
        /// </summary>
        public IEnumerable<Advertising> Adv { get; set; }
        public IEnumerable<StatByAdv> Stat2D { get; set; }
        public string TextPattern { get; set; } = "{A}: {VP:P}";

    }

    public class StatByAdv
    {
        public int Cnt { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }
    }
}
