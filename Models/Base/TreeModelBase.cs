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
        public int? ParentId { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("Dir")]
        [Display(Name = "Директория")]
        public int Dir { get; set; }

    }
}
