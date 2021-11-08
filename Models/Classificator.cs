using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dental.Models
{
    [Table("Classificator")]
    class Classificator : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public ObservableCollection<PriceForClients> PriceForClients { get; set; } 

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
