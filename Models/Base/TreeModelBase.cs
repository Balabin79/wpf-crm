using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;
using Dental.Interfaces;


namespace Dental.Models.Base
{
    abstract class TreeModelBase : ViewModelBase
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ParentId")]
        public int ParentId { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("Dir")]
        [Display(Name = "Директория")]
        public int Dir { get; set; }

        [Column("IsSys")]
        [Display(Name = "Системные значения")]
        public int IsSys { get; set; }

        [Column("IsDelete")]
        [Display(Name = "Удалено")]
        public int IsDelete { get; set; }

        [NotMapped]
        public bool IsExpanded { 
            get {
                if (Id == 1) { _isExpanded = true; }
                return _isExpanded;
            } 

            set => Set(ref _isExpanded, value);
        }

        private bool _isExpanded = false;

    }
}
