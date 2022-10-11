using Dental.Infrastructures.Attributes;
using Dental.Models.Base;
using Dental.Models.Templates;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Nomenclature")]
    public class Nomenclature : BaseTemplate<Nomenclature>
    {
        [Display(Name = "Артикул")]
        [Clonable]
        public string Code
        {
            get { return GetProperty(() => Code); }
            set { SetProperty(() => Code, value?.Trim()); }
        }

        [NotMapped]
        public string FullName { get => string.IsNullOrEmpty(Code) ? Name : Name + " (Код: " + Code + ")"; }

        [Display(Name = "Единица измерения")]
        [Clonable]
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

        [Clonable]
        public int? MeasureId { get; set; }

        [Clonable]
        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public override object Clone()
        {
            return new Nomenclature()
            {
                IsDir = IsDir,
                Name = Name,
                Parent = Parent,
                ParentId = ParentId,
                UpdatedAt = UpdatedAt,
                Code = Code,
                Price = Price,
                Measure = Measure,
                MeasureId = MeasureId
            };
        }
    }
}
