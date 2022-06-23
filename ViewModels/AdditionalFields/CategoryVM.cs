using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Dental.ViewModels.Invoices
{
    public class CategoryVM : BindableBase, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название категории"" обязательно для заполнения")]
        public string Caption
        {
            get { return GetProperty(() => Caption); }
            set { SetProperty(() => Caption, value); }
        }

        [Required(ErrorMessage = @"Поле ""Системное название категории"" обязательно для заполнения")]
        public string SysName
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value); }
        }

        public string Title
        {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

       // public ICollection<Client> Clients { get; set; }
    }
}
