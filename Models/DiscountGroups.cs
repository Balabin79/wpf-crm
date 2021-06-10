using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("DiscountGroups")]
    class DiscountGroups : AbstractBaseModel, IDataErrorInfo, ITreeModel, ITreeViewCollection
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Вид скидки")]
        public string DiscountGroupType { get; set; } = "Процент";

        [Display(Name = "Размер скидки")]
        public float AmountDiscount { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

    }
}
