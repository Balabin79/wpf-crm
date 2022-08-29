using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using Dental.Services;
using System.Data.Entity;
using Dental.Models.Base;
using Dental.Models.Templates;

namespace Dental.ViewModels.Templates
{
    public class TreatmentPlanMediatorVM : BindableBase, IDataErrorInfo
    {
        public TreatmentPlanMediatorVM(ApplicationContext db, int id = 0) => Collection = db?.TreatmentPlans.Where(f => f.IsDir == 1 && f.Id != id).Include(f => f.Parent).OrderBy(f => f.Name).ToObservableCollection() ?? new ObservableCollection<TreatmentPlan>();

        [Required(ErrorMessage = @"Поле ""Название"" обязательно для заполнения")]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public int? IsDir
        {
            get { return GetProperty(() => IsDir); }
            set { SetProperty(() => IsDir, value); }
        }

        public string Guid { get; set; }

        public bool IsVisibleItemForm
        {
            get { return GetProperty(() => IsVisibleItemForm); }
            set { SetProperty(() => IsVisibleItemForm, value); }
        }

        public ObservableCollection<TreatmentPlan> Collection
        {
            get { return GetProperty(() => Collection); }
            set { SetProperty(() => Collection, value); }
        }

        public TreatmentPlan Parent { get; set; }
        public int? ParentId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
