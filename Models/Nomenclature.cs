using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using System.Reflection;
using Dental.Interfaces;

namespace Dental.Models
{
    [Table("Nomenclature")]
    class Nomenclature : AbstractBaseModel, ITreeModel, IDataErrorInfo, ITreeViewCollection
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Артикул")]
        public string VendorCode { get; set; }

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public int? UnitId { get; set; }
        public Unit Unit { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        public string NumberInPack { get; set; } // кол-во в упаковке

        [Display(Name = "Штрих-Код")]
        public string BarCode { get; set; }

        // код, артикул, номенклатурная группа , ед.изм., кол-во при выборе некоторых полей,

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }  
}
