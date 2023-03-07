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
using Dental.Infrastructures.Logs;
using Dental.Services;
using Dental.Models.Base;

namespace Dental.ViewModels.Statistics
{
    public class AdvertisingSourcesViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;

        public AdvertisingSourcesViewModel()
        {
            try
            {
                db = new ConnectToDb().Context;
                IsCnt = true;
                SetPattern();
                SetValueDataMember();
                SetTotalPattern();
                Search();
            }
            catch
            {

            }
        }

        private void SetPattern() => TextPattern = IsCnt ? "{A}: {VP:P}" : "{A}: {V:C}";
        private void SetTotalPattern() => TotalPattern = IsCnt ? "Количество счетов: {TV}" : "Сумма по счетам: {TV:C}";
        private void SetValueDataMember() => Val = IsCnt ? "Cnt" : "Sum";

        [Command]
        public void Switch()
        {
            try
            {
                IsCnt = !IsCnt;
                SetPattern();
                SetTotalPattern();
                SetValueDataMember();
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        public bool IsCnt
        {
            get { return GetProperty(() => IsCnt); }
            set { SetProperty(() => IsCnt, value); }
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


                if (int.TryParse(InvoicesSearchMode?.ToString(), out int paimentStatus)) if (paimentStatus == 2) paimentStatus = 0;

                var invoices = InvoicesSearchMode != null ?             
                    db.Invoices.Include(f => f.InvoiceItems).Include(f => f.Advertising)
                       .Where(f => f.DateTimestamp >= dateFrom && f.DateTimestamp <= dateTo && f.Paid == paimentStatus ).ToArray() :
                     db.Invoices.Include(f => f.InvoiceItems).Include(f => f.Advertising)
                       .Where(f => f.DateTimestamp >= dateFrom && f.DateTimestamp <= dateTo).ToArray(); 


                Stat2D = invoices?.GroupBy(f => f?.Advertising?.Name)?.
                    Select(t => new StatByAdv
                    {
                        Cnt = t.Count(),
                        Name = t.Select(f => f?.Advertising?.Name)?.FirstOrDefault(),
                        Cost = t.Select(f => f?.Advertising?.Cost)?.FirstOrDefault(),
                        Sum = t.SelectMany(f => f.InvoiceItems.Where(i => i.Price > 0 && i.Count > 0)).Sum(f => f.Price * f.Count)
                    })?.ToList();
            }
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// //
        /// </summary>
        public IEnumerable<Advertising> Adv
        {
            get { return GetProperty(() => Adv); }
            set { SetProperty(() => Adv, value); }
        }

        public IEnumerable<StatByAdv> Stat2D
        {
            get { return GetProperty(() => Stat2D); }
            set { SetProperty(() => Stat2D, value); }
        }

        public string TextPattern
        {
            get { return GetProperty(() => TextPattern); }
            set { SetProperty(() => TextPattern, value); }
        }

        public string Val
        {
            get { return GetProperty(() => Val); }
            set { SetProperty(() => Val, value); }
        }

        public string TotalPattern
        {
            get { return GetProperty(() => TotalPattern); }
            set { SetProperty(() => TotalPattern, value); }
        }
    }

    public class StatByAdv
    {
        public int Cnt { get; set; }
        public string Name { get; set; }
        public decimal? Sum { get; set; }
        public decimal? Cost { get; set; }
    }
}
