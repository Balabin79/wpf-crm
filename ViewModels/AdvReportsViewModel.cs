using System.Collections.Generic;
using System.Linq;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class AdvReportsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        
        public AdvReportsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Adv = db.Clients.Include("Advertising").Select(f => f.Advertising).ToArray();
                Stat2D = Adv.GroupBy(f => f.Name).Select(i => new StatByAdv
                {
                    Cnt = i.Count(),
                    Name = i.Select(f => f.Name).FirstOrDefault()
                }).ToList();
            } 
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Отчет по источникам привлечения\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public IEnumerable<Advertising> Adv { get; }
        public IEnumerable<StatByAdv> Stat2D { get; }
       
        private readonly ApplicationContext db;
    }

    public class StatByAdv
    {
        public int Cnt { get; set; }
        public string Name { get; set; }
    }
}

