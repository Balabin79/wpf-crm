using Dental.Models;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.ViewModels.Estimates
{
    public class EstimateMaterialItemVM : BindableBase, IDataErrorInfo
    {
        public Estimate Estimate
        {
            get { return GetProperty(() => Estimate); }
            set { SetProperty(() => Estimate, value); }
        }

        [Required(ErrorMessage = @"Поле ""Материал"" обязательно для заполнения")]
        public Nomenclature Nomenclature
        {
            get { return GetProperty(() => Nomenclature); }
            set { SetProperty(() => Nomenclature, value); }
        }

        public Measure Measure
        {
            get { return GetProperty(() => Measure); }
            set { SetProperty(() => Measure, value); }
        }

        public int Count
        {
            get { return GetProperty(() => Count); }
            set { SetProperty(() => Count, value); }
        }

        public decimal Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
