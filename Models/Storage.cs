using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using System.Reflection;
using Dental.Interfaces;

namespace Dental.Models
{
    [Table("Storage")]
    class Storage : AbstractBaseModel, IDataErrorInfo, ITreeModel, ITreeViewCollection
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public int? EmployeeId { get; set; } // Ответственное лицо
        public Employee Employee { get; set; }

        public int? ParentId { get; set; }
        public int IsDir { get; set; }

        public string Error{ get => string.Empty; }

        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
