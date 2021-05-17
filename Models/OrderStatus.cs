using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("OrderStatuses")]
    class OrderStatus : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
