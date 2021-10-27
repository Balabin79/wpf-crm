using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("WageRateForEmployments")]
    class WageRateForEmployments : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Является базовым")]
        public int IsBasic { get; set; } = 0;       
        
        [Display(Name = "Применяется правило")]
        public int IsApplyRule { get; set; } = 0;

        [Display(Name = "Процент или значение")] // 10% или 500 руб.
        public string PercentOrCost { get; set; }

        [Display(Name = "Значение")]
        public string Value { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
