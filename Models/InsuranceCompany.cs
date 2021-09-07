using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System;

namespace Dental.Models
{
    [Table("InsuranceCompanies")]
    class InsuranceCompany : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [MaxLength(255)]
        [Display(Name = "Код")]
        public string Code { get; set; } = "";

        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address { get; set; } = "";

        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; } = "";

        [Display(Name = "Лицензия")]
        public string License { get; set; } = "";

        [Display(Name = "Дата выдачи")]
        public string StartDateLicense { get; set; } = DateTime.Now.ToShortDateString().ToString();

        [Display(Name = "Дата окончания")]
        public string EndDateLicense { get; set; } = DateTime.Now.ToShortDateString().ToString();

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = "89111111111";

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Дополнительный телефон")]
        public string AdditionalPhone { get; set; } = "88001000000";

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Факс")]
        public string Fax { get; set; } = "89111111111";

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "example@company.com";

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
