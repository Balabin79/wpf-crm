using System;
using System.Collections.Generic;
using System.Linq;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;

using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Services;
using Dental.Infrastructures.Extensions.Notifications;
using System.IO;
using System.Windows.Media.Imaging;
using Dental.Views.WindowForms;

namespace Dental.ViewModels
{
    class ReportsByPeriodsViewModel : ViewModelBase
    {

        public ReportsByPeriodsViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Appointments = db.Appointments
                    .Include(f => f.Service)
                    .Where(f => !string.IsNullOrEmpty(f.StartTime))
                    .GroupBy(f => f.Service.Name)
                    .Select(i => new ServicesCalc() {

                            CalcCost = i.Sum(p => p.CalcCost),
                            CalcPrice = i.Sum(p => p.CalcPrice)
                    }
                        )
                    .ToArray();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Действия пользователя\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public IEnumerable<ServicesCalc> Appointments { get; }

        ApplicationContext db;

        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public int PeriodType { get; set; }
    }


    public class ServicesCalc
    {
        public decimal? CalcPrice { get; set; }
        public decimal? CalcCost { get; set; }
        public string Service { get; set; }
    }
}