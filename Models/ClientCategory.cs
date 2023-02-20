using Dental.Infrastructures.Attributes;
using Dental.Models.Base;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace Dental.Models
{
    [Table("ClientCategories")]
    public class ClientCategory : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название"" обязательно для заполнения")]
        public string Name { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
