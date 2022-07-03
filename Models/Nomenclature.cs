using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Nomenclature")]
    public class Nomenclature : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value?.Trim();
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _Name;

        [Display(Name = "Артикул")]
        public string Code
        {
            get => _Code;
            set
            { 
                _Code = value?.Trim();
                OnPropertyChanged(nameof(Code));
            }
        }
        private string _Code;

        [NotMapped]
        public string FullName { get => string.IsNullOrEmpty(Code) ? Name : Name + " (Код: " + Code + ")"; }

        [Display(Name = "Единица измерения")]
        public Measure Measure 
        {
            get => measure;
            set
            {
                measure = value;
                OnPropertyChanged(nameof(Measure));
            }
        }
        private Measure measure;
        public int? MeasureId { get; set; }

        public decimal? Price
        {
            get => _Price;
            set => _Price = value;
        }
        private decimal? _Price;

        public Nomenclature Parent
        {
            get => parent;
            set
            {
                parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }
        private Nomenclature parent;
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
