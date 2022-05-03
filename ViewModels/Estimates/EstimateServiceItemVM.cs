using Dental.Models;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.ViewModels.Estimates
{
    class EstimateServiceItemVM : BindableBase, IDataErrorInfo
    {
        public Estimate Estimate
        {
            get { return GetProperty(() => Estimate); }
            set { SetProperty(() => Estimate, value); }
        }

        [Required(ErrorMessage = @"Поле ""Название сметы"" обязательно для заполнения")]
        public Service Service
        {
            get { return GetProperty(() => Service); }
            set { SetProperty(() => Service, value); }
        }

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
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
