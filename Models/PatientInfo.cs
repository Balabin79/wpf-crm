using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("PatientInfo")]
    public class PatientInfo : AbstractBaseModel
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
        [Display(Name = "Моб.Телефон")]
        public string MobPhone { get; set; }

        [Phone]
        [Display(Name = "Дом.Телефон")]
        public string HomePhone { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public int? AreaId { get; set; } // местность - городская или сельская
        public int? MaritalStatusId { get; set; } // семейное положение
        public int? MainEducationId { get; set; } // общее образование
        public int? ProfEducationId { get; set; } // профессиональное образование
        public int? EmploymentId { get; set; } // занятость

        [Display(Name = "Номер медицинской карты")]
        public string MedicalCardNumber { get; set;  } //номер карты

        [Display(Name = "Дата создания медицинской карты")]
        public string MedicalCardCreatedAt { get; set;  } //дата заполнения карты

        [Display(Name = "Место работы")]
        public string PlaceOfWork { get; set; } // Место работы

        [Display(Name = "СНИЛС")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [MaxLength(11, ErrorMessage = "Длина не более 11 цифр")]
        public string Snils { get; set; }

        [Display(Name = "Серия паспорта")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [MinLength(4, ErrorMessage = "Длина не менее 4 цифр")]
        [MaxLength(4, ErrorMessage = "Длина не более 4 цифр")]
        public string SerialNumberPassport { get; set; } //серия паспорта

        [Display(Name = "Номер паспорта")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [MinLength(4, ErrorMessage = "Длина не менее 6 цифр")]
        [MaxLength(4, ErrorMessage = "Длина не более 6 цифр")]
        public string NumberPassport { get; set; } // номер паспорта

        [Display(Name = "Выдан")]
        public string DateIssuedPassport { get; set; } // дата выдачи

        [Display(Name = "Код категории льготы")]
        public string BenefitCategoryCode { get; set; } // Код категории льготы
                                                        // 
                                                        // Диагноз, установленный направившей медициснкой организацией:
        [Display(Name = "Основной диагноз")]
        public string MainDiagnos { get; set; }   // основной

        [Display(Name = "код по МКБ-10")]
        public string MKBCode { get; set; }   // код по МКБ-10 

        [Display(Name = "Осложнения основного")]
        public string ComplicationsMainDiagnosis { get; set; }   // осложнения основного 

        [NotMapped]
        [Display(Name = "Полис ОМС")]
        public MedicalPolicy PolisOMS { get; set; } // Полис ОМС

        [NotMapped]
        [Display(Name = "Адрес")]
        public string FullAddress{ get; set; } 
    }
}
