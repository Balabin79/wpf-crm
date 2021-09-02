using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dental.Models
{
    [Table("Organizations")]
    class Organization : AbstractBaseModel, IDataErrorInfo
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName { get; set; }

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; }

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp { get; set; }

        // Общая информация
        [Display(Name = "Лого")]
        public string Logo { get; set; }

        [NotMapped]
        public ImageSource Image { get; set; }


        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address { get; set; }

        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // Банковские реквизиты
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik { get; set; }

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber { get; set; }

        [Display(Name = "Наименование банка")]
        public string BankName { get; set; }

        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn { get; set; }

        [Display(Name = "Дата регистрации")]
        public string RegisterDate { get; set; }

        [Display(Name = "Генеральный директор")]
        public string GeneralDirector { get; set; }

        [Display(Name = "Лицензия")]
        public string License { get; set; }

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy { get; set; }

        [Display(Name = "Дополнительно")]
        public string Additional { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }  
}
