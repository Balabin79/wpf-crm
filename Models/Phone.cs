using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Phones")]
    class Phone : AbstractBaseModel
    {
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Number { get; set; } = "9111111111";
    }
}
