using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;

namespace Dental.ViewModels
{
    class AdvReportsViewModel : ViewModelBase
    {
        
        public AdvReportsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Adv = db.PatientInfo.Include("Advertising").Select(f => f.Advertising).ToArray();
                Stat2D = Adv.GroupBy(f => f.Name).Select(i => new StatByAdv
                {
                   // Id = i.Key,
                    Cnt = i.Count(),
                    Name = i.Select(f => f.Name).FirstOrDefault()
                }).ToList();
                //Collection = db.UserActions.OrderBy(f => f.CreatedAt).ToArray();
            } catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Действия пользователя\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public IEnumerable<Advertising> Adv { get; }
        public IEnumerable<StatByAdv> Stat2D { get; }
       
        ApplicationContext db;
    }

    public class StatByAdv
    {
        //public int Id { get; set; }
        public int Cnt { get; set; }
        public string Name { get; set; }
    }
}

