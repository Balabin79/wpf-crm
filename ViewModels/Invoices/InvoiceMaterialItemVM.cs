using Dental.Models;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.ViewModels.Invoices
{
    public class InvoiceMaterialItemVM : BindableBase, IDataErrorInfo
    {
        public Invoice Invoice
        {
            get { return GetProperty(() => Invoice); }
            set { SetProperty(() => Invoice, value); }
        }

        [Required(ErrorMessage = @"Поле ""Материал"" обязательно для заполнения")]
        public Nomenclature Nomenclature
        {
            get { return GetProperty(() => Nomenclature); }
            set { SetProperty(() => Nomenclature, value); }
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
