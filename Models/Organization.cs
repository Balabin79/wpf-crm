using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dental.Models
{
    [Table("Organizations")]
    public class Organization : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name { get; set; } = "Новая организация";

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName { get; set; } = "";

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; } = "";

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp { get; set; } = "";

        public string Logo { get; set; }

        /*
        ImageSource image = new BitmapImage(new Uri("c:\\Users\\user\\source\\repos\\Dental\\Resources\\Icons\\example\\cyprus.jpg"));
        [NotMapped]
        public ImageSource Logo {
            get => image;
            set => image = value; 
        }
        */



            byte[] imageData;

            [NotMapped]
            public byte[] ImageData { 
                get => !String.IsNullOrEmpty(Logo) ? File.ReadAllBytes(Logo) : File.ReadAllBytes("c:\\Users\\user\\source\\repos\\Dental\\Resources\\Icons\\example\\cyprus.jpg");
                set => imageData = value;
              }

            private BitmapImage imageSource;

            [NotMapped]
            public BitmapImage LogoEdit
            {
                get
                {
                    if (imageSource == null) SetImageSource();
                    return imageSource;                
                }
                set
                {
                    imageSource = value;
                }
            }
            void SetImageSource()
            {
                var stream = new MemoryStream(ImageData);
                imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = stream;
                imageSource.EndInit();
            }



            // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address { get; set; } = "";

        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; } = "";

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = "89111111111";

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "example@company.com";


        // Банковские реквизиты
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik { get; set; } = "";

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber { get; set; } = "";

        [Display(Name = "Наименование банка")]
        public string BankName { get; set; } = "";

        // Регистрационная информация
        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn { get; set; } = "";

        [Display(Name = "Дата регистрации")]
        public string RegisterDate { get; set; } = DateTime.Now.ToShortDateString().ToString();

        [Display(Name = "Генеральный директор")]
        public string GeneralDirector { get; set; } = "";

        [Display(Name = "Лицензия")]
        public string License { get; set; } = "";

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy { get; set; } = "";


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
                    case "LegalAddress": return item.LegalAddress == LegalAddress;
                    case "Phone": return item.Phone == Phone;
                    case "Email": return item.Email == Email;
                    case "Bik": return item.Bik == Bik;
                    case "AccountNumber": return item.AccountNumber == AccountNumber;
                    case "BankName": return item.BankName == BankName;
                    case "RegisterDate": return item.RegisterDate == RegisterDate;
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
             LegalAddress = copy.LegalAddress;
             Phone = copy.Phone;
             Email = copy.Email;
             Bik = copy.Bik;
             AccountNumber = copy.AccountNumber;
             BankName = copy.BankName;
             RegisterDate = copy.RegisterDate;
             Ogrn = copy.Ogrn;
             GeneralDirector = copy.GeneralDirector;
             License = copy.License;
             WhoIssuedBy = copy.WhoIssuedBy;
        }
    }
}
