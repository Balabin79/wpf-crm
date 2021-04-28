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

        [NotMapped]
        public DevExpress.XtraPrinting.Drawing.ImageSource  Image{ get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = "Максимальная длина строки не более 255 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = "Максимальная длина строки не более 255 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(255, ErrorMessage = "Максимальная длина строки не более 255 символов")]
        public string MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; } = DateTime.Now.ToShortDateString().ToString();

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? FirstName + " " + LastName : FirstName + " " + MiddleName + " " + LastName;
        }

    }

}
