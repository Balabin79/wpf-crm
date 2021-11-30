using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Classificator")]
    public class Classificator : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set => _Name = value?.Trim();
        }
        private string _Name;

        [Display(Name = "Код")]
        public string Code
        {
            get => _Code;
            set => _Code = value?.Trim();
        }
        private string _Code;

        [NotMapped]
        public string FullName { get => string.IsNullOrEmpty(Code) ? Name : Name + " (Код: " + Code + ")"; }
    
        public string Price
        {
            get => _Price;
            set => _Price = value?.Trim();
        }
        private string _Price;

        public string Cost
        {
            get => _Cost;
            set => _Cost = value?.Trim();
        }
        private string _Cost;

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


        public override string ToString()
        {
            return Name;
        }
    }
}
