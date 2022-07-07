using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.Entity;
using DevExpress.Mvvm.Native;

namespace Dental.ViewModels.AdditionalFields
{
    public class FieldVM : BindableBase, IDataErrorInfo
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = @"Поле ""Название поля"" обязательно для заполнения")]
        public string Label
        {
            get { return GetProperty(() => Label); }
            set { SetProperty(() => Label, value); }
        }

        [Required(ErrorMessage = @"Поле ""Системное название поля"" обязательно для заполнения")]
        public string SysName
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value); }
        }

        public TemplateType TypeValue
        {
            get { return GetProperty(() => TypeValue); }
            set { SetProperty(() => TypeValue, value); }
        }
        public int? TypeValueId { get; set; }

        public string Guid { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }

    
}
