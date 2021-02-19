using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("Organizations")]
    class Organization : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name { get; set; } 

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName { get; set; }

        [MaxLength(10, ErrorMessage ="Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; }

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp { get; set; } 

        [Display(Name = "Лого")]
        public string Logo { get; set; }

        // Контактная инф-ция
        [Display(Name = "Юридический адрес")]
        public string Address { get; set; }

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
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

        // Регистрационная информация
        [Display(Name = "Свидетельство")]
        public string Сertificate { get; set; }

        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn { get; set; }

        [Display(Name = "Генеральный директор")]
        public string GeneralDirector { get; set; } 

        [Display(Name = "Лицензия")]
        public string License { get; set; } 

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy { get; set; }

        
        public bool this[PropertyInfo prop, Organization item]
        {
            get
            {
                switch(prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name" : return item.Name == Name;
                    case "ShortName": return item.ShortName == ShortName;
                    case "Inn": return item.Inn == Inn;
                    case "Kpp": return item.Kpp == Kpp;
                    case "Logo" : return item.Logo == Logo;
                    case "Address": return item.Address == Address;
                    case "Phone": return item.Phone == Phone;
                    case "Email": return item.Email == Email;
                    case "Bik": return item.Bik == Bik;
                    case "AccountNumber": return item.AccountNumber == AccountNumber;
                    case "BankName": return item.BankName == BankName;
                    case "Сertificate": return item.Сertificate == Сertificate;
                    case "Ogrn": return item.Ogrn == Ogrn;
                    case "GeneralDirector": return item.GeneralDirector == GeneralDirector;
                    case "License": return item.License == License;
                    case "WhoIssuedBy": return item.WhoIssuedBy == WhoIssuedBy;
                    default: return true;
                }
            }         
        }

        public void Copy(Organization copy)
        {
             Name = copy.Name;
             ShortName = copy.ShortName;
             Inn = copy.Inn;
             Kpp = copy.Kpp;
             Logo = copy.Logo;
             Address = copy.Address;
             Phone = copy.Phone;
             Email = copy.Email;
             Bik = copy.Bik;
             AccountNumber = copy.AccountNumber;
             BankName = copy.BankName;
             Сertificate = copy.Сertificate;
             Ogrn = copy.Ogrn;
             GeneralDirector = copy.GeneralDirector;
             License = copy.License;
             WhoIssuedBy = copy.WhoIssuedBy;
        }
    }
}
