using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("AdditionalEmployeeFields")]
    public class AdditionalEmployeeField : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название поля"" обязательно для заполнения")]
        public string Label
        {
            get => label;
            set
            {
                label = value;
                OnPropertyChanged(nameof(Label));
            }
        }
        private string label;

        [Required(ErrorMessage = @"Поле ""Системное имя поля"" обязательно для заполнения")]
        public string SysName
        {
            get => sysName;
            set
            {
                sysName = value;
                OnPropertyChanged(nameof(SysName));
            }
        }
        private string sysName;

        public TemplateType TypeValue
        {
            get => typeValue;
            set
            {
                typeValue = value;
                OnPropertyChanged(nameof(typeValue));
            }
        }
        public int? TypeValueId { get; set; }
        private TemplateType typeValue;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
    }
}
