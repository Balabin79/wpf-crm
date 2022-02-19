﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dental.Models;
using System.Windows;
using DevExpress.Xpf.Core;

namespace Dental.ViewModels
{
    class ReportsByEmpViewModel : ViewModelBase
    {

        public ReportsByEmpViewModel()
        {
            try
            {
                db = new ApplicationContext();
                ServicesItems = db.TreatmentPlanItems.Include("Employee").Include("Classificator").ToArray();

                Employes = ServicesItems.GroupBy(f => f.EmployeeId).Select(i => new Employes
                {
                    Name = i.Select(f => f.Employee?.Fio).FirstOrDefault(),
                    Prices = i.Select(f => f.Classificator?.Price).Sum(),
                    Costs = i.Select(f => f.Classificator?.Cost).Sum(),

                }).ToList();
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Действия пользователя\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public IEnumerable<TreatmentPlanItems> ServicesItems { get; }
        public IEnumerable<Employes> Employes { get; }

        ApplicationContext db;
    }

    public class Employes
    {
        public decimal? Prices { get; set; }
        public decimal? Costs { get; set; }
        public string Name { get; set; }
    }
}