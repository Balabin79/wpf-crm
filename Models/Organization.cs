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
    public class Organization : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name 
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName 
        {
            get => _ShortName;
            set
            {
                _ShortName = value;
                OnPropertyChanged(nameof(ShortName));
            }
        }

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn
        {
            get => _Inn;
            set
            {
                _Inn = value;
                OnPropertyChanged(nameof(Inn));
            }
        }

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp 
        {
            get => _Kpp;
            set
            {
                _Kpp = value;
                OnPropertyChanged(nameof(Kpp));
            }
        }

        // Общая информация
        [Display(Name = "Лого")]
        public string Logo 
        {
            get => _Logo;
            set
            {
                _Logo = value;
                OnPropertyChanged(nameof(Logo));
            }
        }

        [NotMapped]
        public ImageSource Image { get; set; }



        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address 
        {
            get => _Address;
            set
            {
                _Address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        [Display(Name = "Юридический адрес")]
        public string LegalAddress 
        {
            get => _LegalAddress;
            set
            {
                _LegalAddress = value;
                OnPropertyChanged(nameof(LegalAddress));
            }
        }

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone 
        {
            get => _Phone;
            set
            {
                _Phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value;
                OnPropertyChanged(nameof(Email));
            }
        }


        // Банковские реквизиты
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik 
        {
            get => _Bik;
            set
            {
                _Bik = value;
                OnPropertyChanged(nameof(Bik));
            }
        }

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber { 
            get => _AccountNumber;
            set 
            {
                _AccountNumber = value;
                OnPropertyChanged(nameof(AccountNumber)); 
            } 
        }

        [Display(Name = "Наименование банка")]
        public string BankName
        {
            get => _BankName;
            set
            {
                _BankName = value;
                OnPropertyChanged(nameof(BankName));
            }
        }

        // Регистрационная информация
        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn 
        {
            get => _Ogrn;
            set
            {
                _Ogrn = value;
                OnPropertyChanged(nameof(Ogrn));
            }
        }

        [Display(Name = "Дата регистрации")]
        public string RegisterDate
        {
            get
            {
                try
                {

                    DateTime.TryParse(_RegisterDate, out DateTime v);
                    if (string.IsNullOrEmpty(_RegisterDate)) return DateTime.Now.ToShortDateString().ToString();
                    return v.ToShortDateString().ToString();
                }
                catch (Exception e)
                {
                    return DateTime.Now.ToShortDateString();
                }

            }
            set
            {
                _RegisterDate = value;
                OnPropertyChanged(nameof(RegisterDate));
            }
        }
       
        [Display(Name = "Генеральный директор")]
        public string GeneralDirector 
        {
            get => _GeneralDirector;
            set
            {
                _GeneralDirector = value;
                OnPropertyChanged(nameof(GeneralDirector));
            }
        }

        [Display(Name = "Лицензия")]
        public string License 
        {
            get => _License;
            set
            {
                _License = value;
                OnPropertyChanged(nameof(License));
            }
        }

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy 
        {
            get => _WhoIssuedBy;
            set
            {
                _WhoIssuedBy = value;
                OnPropertyChanged(nameof(WhoIssuedBy));
            }
        }

        [Display(Name = "Дополнительно")]
        public string Additional 
        {
            get => _Additional;
            set
            {
                _Additional = value;
                OnPropertyChanged(nameof(Additional));
            }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public string this[PropertyInfo property]
        {
            get
            {
                switch (property.Name)
                {
                    case "Name": return Name;
                    case "ShortName": return ShortName;
                    case "Inn": return Inn;
                    case "Kpp": return Kpp;
                    case "Ogrn": return Ogrn;
                    case "Logo": return Logo;
                    case "Address": return Address;
                    case "LegalAddress": return LegalAddress;
                    case "Email": return Email;
                    case "Phone": return Phone;
                    case "Bik": return Bik;
                    case "AccountNumber": return AccountNumber;
                    case "BankName": return BankName;
                    case "RegisterDate": return RegisterDate;
                    case "GeneralDirector": return GeneralDirector;
                    case "License": return License;
                    case "WhoIssuedBy": return WhoIssuedBy;
                    case "Additional": return Additional;
                    default: return null;
                }
            }
        }

        public void Copy(Organization copy)
        {
            Id = copy.Id;
            Name = copy.Name;
            ShortName = copy.ShortName;
            Inn = copy.Inn;
            Kpp = copy.Kpp;
            Ogrn = copy.Ogrn;
            Logo = copy.Logo;
            Email = copy.Email;
            Address = copy.Address;
            LegalAddress = copy.LegalAddress;
            Phone = copy.Phone;
            Email = copy.Email;
            Bik = copy.Bik;
            AccountNumber = copy.AccountNumber;
            BankName = copy.BankName;
            RegisterDate = copy.RegisterDate;
            GeneralDirector = copy.GeneralDirector;
            License = copy.License;
            WhoIssuedBy = copy.WhoIssuedBy;
            Additional = copy.Additional;
        }

        private string _AccountNumber;
        private string _Name;
        private string _ShortName;
        private string _Inn;
        private string _Kpp;
        private string _Ogrn;
        private string _Logo;
        private string _Email;
        private string _Address;
        private string _LegalAddress;
        private string _Phone;
        private string _Bik;
        private string _Additional;
        private string _BankName;
        private string _GeneralDirector;
        private string _License;
        private string _WhoIssuedBy;
        private string _RegisterDate;

        public event PropertyChangedEventHandler PropertyChanged;         
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }  
}
