using Dental.Models.Share;
using Dental.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.PatientCard
{
    [Table("PatientInfo")]
    class PatientInfo : User
    {
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

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

        [Display(Name = "Полис ОМС")]
        public MedicalPolicy PolisOMS { get; set; } // Полис ОМС

        [Display(Name = "Место регистрации")]
        public UserLocation Location { get; set; } // Место проживания
    }
}
