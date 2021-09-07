using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("PatientInfo")]
    class PatientInfo : AbstractBaseModel
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Фамилия"" не более 255 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; }

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? FirstName + " " + LastName : FirstName + " " + MiddleName + " " + LastName;
        }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Skype")]
        [MaxLength(255)]
        public string Skype { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Номер медицинской карты")]
        public string MedicalCardNumber { get; set;  } //номер карты

        [Display(Name = "Дата создания медицинской карты")]
        public string MedicalCardCreatedAt { get; set;  } //дата заполнения карты

        [Display(Name = "Серия и номер паспорта")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string SerialNumberPassport { get; set; } //серия и номер паспорта

        [Display(Name = "Дата выдачи")]
        public string DateIssuedPassport { get; set; } // дата выдачи

        [Display(Name = "Кем выдан")]
        public string IssuedByPassport { get; set; } // кем выдан

        [Display(Name = "Адрес")]
        public string Address{ get; set; }

        [Display(Name = "Комментарий")]
        public string Note { get; set; }
    }
}
