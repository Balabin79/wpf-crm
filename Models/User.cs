using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;

namespace Dental.Models
{


    abstract class User : AbstractBaseModel
    {
        // Общая информация
       [Display(Name = "Фото")]
       public string Photo { get; set; }

        [Display(Name = "Имя")]
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = "Иван";

        [Display(Name = "Фамилия")]
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = "Иванов";

        [Display(Name = "Отчество")]
        [MaxLength(255)]
        public string MiddleName { get; set; } = "";

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; } = DateTime.Now.ToShortDateString().ToString();

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? FirstName + " " + LastName : FirstName + " " + MiddleName + " " + LastName;
        }

    }

}
