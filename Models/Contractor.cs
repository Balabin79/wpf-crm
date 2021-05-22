using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using DevExpress.Mvvm;
using System.Reflection;
using Dental.Models.Share;

namespace Dental.Models
{
    [Table("Contractors")]
    class Contractor : AbstractBaseModel, IDataErrorInfo
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Групппа контрагентов")]
        public int? ContractorsGroupId { get; set; }
        public ContractorGroup ContractorsGroup { get; set; }

        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email { get; set; }

        public string Skype { get; set; }

        [Display(Name = "Моб.телефон")]
        [Phone(ErrorMessage = @"В поле ""Мобильный телефон"" введено некорректное значение")]
        public string MobilePhone { get; set; }

        [Display(Name = "Раб.телефон")]
        [Phone(ErrorMessage = @"В поле ""Рабочий телефон"" введено некорректное значение")]
        public string WorkPhone { get; set; }

        public string ActualAddress { get; set; } // Фактический адрес
        public string LegalAddress { get; set; } // Юридический адрес

        // Юридическая информация
        [MinLength(10, ErrorMessage = "Длина не менее 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; }

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp { get; set; }

        [Display(Name = "ОКПО")]
        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okpo { get; set; }



        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn { get; set; }

        [Display(Name = "Номер свидетельства")]
        public string СertificateNumber { get; set; }


        /********** Банковские реквизиты ***********/
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik { get; set; }

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber { get; set; }

        [Display(Name = "Корреспондирующий счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string CorrNumber { get; set; }

        [Display(Name = "Наименование банка")]
        public string BankName { get; set; }


        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return IDataErrorInfoHelper.GetErrorText(this, columnName);
            }
        }
    }
}
