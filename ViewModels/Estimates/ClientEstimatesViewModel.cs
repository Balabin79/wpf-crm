using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Models.Base;
using DevExpress.Xpf.Grid;
using Dental.Views.WindowForms;
using Dental.Services;
using Dental.Views.Estimates;
using Dental.Infrastructures.Converters;
using Dental.Infrastructures.Extensions.Notifications;
using DevExpress.Mvvm.DataAnnotations;
using Dental.ViewModels.Estimates;
using System.Text;
using System.Threading.Tasks;


namespace Dental.ViewModels.Estimates
{
    class ClientEstimatesViewModel : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public ClientEstimatesViewModel()
        {
            try
            {
                db = new ApplicationContext();
                Collection = db.Estimates
                    .Include(f => f.Client)
                    .Include(f => f.EstimateServiseItems.Select(x => x.Employee))
                    .Include(f => f.EstimateServiseItems.Select(x => x.Service))
                    .Include(f => f.EstimateMaterialItems.Select(x => x.Measure))
                    .Include(f => f.EstimateMaterialItems.Select(x => x.Nomenclature))
                    .ToObservableCollection();


                /*db.Estimates.Where(f => f.ClientId == Client.Id)
                    .Include(f => f.EstimateServiseItems.Select(x => x.Employee))
                    .Include(f => f.EstimateServiseItems.Select(x => x.Service))
                    .Include(f => f.EstimateMaterialItems.Select(x => x.Measure))
                    .Include(f => f.EstimateMaterialItems.Select(x => x.Nomenclature))
                .ToObservableCollection();*/
            }
            catch (Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно с разделом \"Сметы\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        public ICollection<Estimate> Collection { get; set; }
    }
}
