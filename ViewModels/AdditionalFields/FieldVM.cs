using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Dental.ViewModels.Invoices
{
    public class FieldVM : BindableBase, IDataErrorInfo
    {

        public AdditionalFieldsCategory AdditionalFieldsCategory
        {
            get { return GetProperty(() => AdditionalFieldsCategory); }
            set { SetProperty(() => AdditionalFieldsCategory, value); }
        }
        public int? AdditionalFieldsCategoryId { get; set; }

        [Required(ErrorMessage = @"Поле ""Название поля"" обязательно для заполнения")]
        public string Caption
        {
            get { return GetProperty(() => Caption); }
            set { SetProperty(() => Caption, value); }
        }

        [Required(ErrorMessage = @"Поле ""Системное название поля"" обязательно для заполнения")]
        public string SysName
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value); }
        }

        public string TypeValue
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public string[] TypeValues { get; } = new string[] { "Строка", "Дата", "Целое число", "Дробное число", "Денежное значение", "Галочка"};

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

       // public ICollection<Client> Clients { get; set; }
    }
}
