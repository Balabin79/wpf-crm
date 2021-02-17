using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("EmployeeStatus")]
    class EmployeeStatus : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }  
    }
}
