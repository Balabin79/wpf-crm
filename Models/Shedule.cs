using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Shedule")]
    class Shedule : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        public string TimeOfReceipt { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
