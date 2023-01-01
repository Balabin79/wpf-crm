using Dental.Models;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                Adv = db.Invoices.Include("Advertising")?.Select(f => f.Advertising).ToArray();
                Stat2D = Adv?.GroupBy(f => f?.Name)?.Select(i => new StatByAdv
                {
                    // Id = i.Key,
                    Cnt = i.Count(),
                    Name = i.Select(f => f?.Name)?.FirstOrDefault()
                })?.ToList();
                //Collection = db.UserActions.OrderBy(f => f.CreatedAt).ToArray();
            }
            catch
            {

            }
        }

        public IEnumerable<Advertising> Adv { get; }
        public IEnumerable<StatByAdv> Stat2D { get; }
    }

    public class StatByAdv
    {
        //public int Id { get; set; }
        public int Cnt { get; set; }
        public string Name { get; set; }
    }
}
