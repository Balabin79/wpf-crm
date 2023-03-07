using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("AdditionalClientFields")]
    public class AdditionalClientField : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название поля"" обязательно для заполнения")]
        public string Label 
        {
            get { return GetProperty(() => Label); }
            set { SetProperty(() => Label, value?.Trim()); }
        }

        [Required(ErrorMessage = @"Поле ""Системное имя поля"" обязательно для заполнения")]
        public string SysName
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value?.Trim()); }
        }

        public TemplateType TypeValue
        {
            get { return GetProperty(() => TypeValue); }
            set { SetProperty(() => TypeValue, value); }
        }
        public int? TypeValueId { get; set; }

        public int? Sort
        {
            get { return GetProperty(() => Sort); }
            set { SetProperty(() => Sort, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
    }
}
