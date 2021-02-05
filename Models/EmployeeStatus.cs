using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

namespace Dental.Models
{
    [Table("EmployeeStatus")]
    class EmployeeStatus : ViewModelBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }  
    }
}
