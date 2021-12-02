using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Organizations")]
    public class Organization : AbstractBaseModel, IDataErrorInfo
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name 
        {
            get => _Name;
            set => _Name = value?.Trim();
        }
        private string _Name;

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName 
        {
            get => _ShortName; 
            set => _ShortName = value?.Trim();
        }
        private string _ShortName;

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn
        {
            get => _Inn;
            set => _Inn = value?.Trim();
        }
        private string _Inn;

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp
        {
            get => _Kpp;
            set => _Kpp = value?.Trim();
        }
        private string _Kpp;
        // Общая информация
       
        [Display(Name = "Лого")]
        public string Logo
        {
            get => _Logo;
            set => _Logo = value?.Trim();
        }
        private string _Logo;       

        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address
        {
            get => _Address;
            set => _Address = value?.Trim();
        }
        private string _Address;

        [Display(Name = "Юридический адрес")]
        public string LegalAddress
        {
            get => _LegalAddress;
            set => _LegalAddress = value?.Trim();
        }
        private string _LegalAddress;

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone
        {
            get => _Phone;
            set => _Phone = value?.Trim();
        }
        private string _Phone;

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email
        {
            get => _Email;
            set => _Email = value?.Trim();
        }
        private string _Email;

        // Банковские реквизиты
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik
        {
            get => _Bik;
            set => _Bik = value?.Trim();
        }
        private string _Bik;

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber
        {
            get => _AccountNumber;
            set => _AccountNumber = value?.Trim();
        }
        private string _AccountNumber;

        [Display(Name = "Наименование банка")]
        public string BankName
        {
            get => _BankName;
            set => _BankName = value?.Trim();
        }
        private string _BankName;

        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn
        {
            get => _Ogrn;
            set => _Ogrn = value?.Trim();
        }
        private string _Ogrn;

        [Display(Name = "Дата регистрации")]
        public string RegisterDate
        {
            get => _RegisterDate;
            set => _RegisterDate = value?.Trim();
        }
        private string _RegisterDate;

        [Display(Name = "Генеральный директор")]
        public string GeneralDirector
        {
            get => _GeneralDirector;
            set => _GeneralDirector = value?.Trim();
        }
        private string _GeneralDirector;

        [Display(Name = "Лицензия")]
        public string License
        {
            get => _License;
            set => _License = value?.Trim();
        }
        private string _License;

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy
        {
            get => _WhoIssuedBy;
            set => _WhoIssuedBy = value?.Trim();
        }
        private string _WhoIssuedBy;

        [Display(Name = "Дополнительно")]
        public string Additional
        {
            get => _Additional;
            set => _Additional = value?.Trim();
        }
        private string _Additional;

        [Display(Name = "ОКВЭД")]
        [MaxLength(6, ErrorMessage = "Длина не более 6 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okved
        {
            get => _Okved;
            set => _Okved = value?.Trim();
        }
        private string _Okved;

        [Display(Name = "ОКПО")]
        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okpo
        {
            get => _Okpo;
            set => _Okpo = value?.Trim();
        }
        private string _Okpo;

        [Display(Name = "Корреспондирующий счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string CorrAccountNumber
        {
            get => _CorrAccountNumber;
            set => _CorrAccountNumber = value?.Trim();
        }
        private string _CorrAccountNumber;

        [Display(Name = "Сайт")]
        public string Site
        {
            get => _Site;
            set => _Site = value?.Trim();
        }
        private string _Site;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


        public object Clone()
        {

            return new Organization
            {
                Name = this.Name,
                ShortName = this.ShortName,
                Kpp = this.Kpp,
                Inn = this.Inn,
                Logo = this.Logo,
                Address = this.Address,
                LegalAddress = this.LegalAddress,
                Phone = this.Phone,
                Email = this.Email,
                AccountNumber = this.AccountNumber,
                CorrAccountNumber = this.CorrAccountNumber,
                Bik = this.Bik,
                BankName = this.BankName,
                Ogrn = this.Ogrn,
                RegisterDate = this.RegisterDate,
                GeneralDirector = this.GeneralDirector,
                License = this.License,
                Site = this.Site,
                WhoIssuedBy = this.WhoIssuedBy,
                Additional = this.Additional,
                Okpo = this.Okpo,
                Okved = this.Okved,
            };

            /*Company company = new Company { Name = this.Work.Name };
            return new Person
            {
                Name = this.Name,
                Age = this.Age,
                Work = company
            };*/
        }

        public Organization Copy(Organization model)
        {
            model.Name = this.Name;
            model.ShortName = this.ShortName;
            model.Kpp = this.Kpp;
            model.Inn = this.Inn;
            model.Logo = this.Logo;
            model.Address = this.Address;
            model.LegalAddress = this.LegalAddress;
            model.Phone = this.Phone;
            model.Email = this.Email;
            model.AccountNumber = this.AccountNumber;
            model.CorrAccountNumber = this.CorrAccountNumber;
            model.Bik = this.Bik;
            model.BankName = this.BankName;
            model.Ogrn = this.Ogrn;
            model.RegisterDate = this.RegisterDate;
            model.GeneralDirector = this.GeneralDirector;
            model.License = this.License;
            model.Site = this.Site;
            model.WhoIssuedBy = this.WhoIssuedBy;
            model.Additional = this.Additional;
            model.Okpo = this.Okpo;
            model.Okved = this.Okved;
            return model;
        }


        public override bool Equals(object other)
        {

            //Последовательность проверки должна быть именно такой.
            //Если не проверить на null объект other, то other.GetType() может выбросить //NullReferenceException.            
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            //Если класс находится на вершине иерархии или просто не имеет наследников, то можно просто
            //сделать Vehicle tmp = other as Vehicle; if(tmp==null) return false; 
            //Затем вызвать экземплярный метод, сразу передав ему объект tmp.
            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as Organization);
        }
        public bool Equals(Organization other)
        {
            if (FieldsChanges != null) FieldsChanges = new List<string>();
            NotIsChanges = true;
            if (other == null)
                return false;

            //Здесь сравнение по ссылкам необязательно.
            //Если вы уверены, что многие проверки на идентичность будут отсекаться на проверке по ссылке - //можно имплементировать.
            if (object.ReferenceEquals(this, other))
                return true;

            //Если по логике проверки, экземпляры родительского класса и класса потомка могут считаться равными,
            //то проверять на идентичность необязательно и можно переходить сразу к сравниванию полей.
            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Name, other.Name, "Наименование");
            StringParamsIsEquel(this.ShortName, other.ShortName, "Сокращенное наименование");
            StringParamsIsEquel(this.Kpp, other.Kpp, "КПП");
            StringParamsIsEquel(this.Inn, other.Inn, "ИНН");
            StringParamsIsEquel(this.Logo, other.Logo, "Логотип");
            StringParamsIsEquel(this.Phone, other.Phone, "Телефон");
            StringParamsIsEquel(this.Email, other.Email, "Email");
            StringParamsIsEquel(this.Address, other.Address, "Фактический адрес");
            StringParamsIsEquel(this.LegalAddress, other.LegalAddress, "Юридический адрес");
            StringParamsIsEquel(this.Bik, other.Bik, "БИК");
            StringParamsIsEquel(this.AccountNumber, other.AccountNumber, "Расчетный счет");
            StringParamsIsEquel(this.BankName, other.BankName, "Наименование банка");
            StringParamsIsEquel(this.Ogrn, other.Ogrn, "ОГРН");
            StringParamsIsEquel(this.Okpo, other.Okpo, "ОКПО");
            StringParamsIsEquel(this.Okved, other.Okved, "ОКВЭД");
            StringParamsIsEquel(this.RegisterDate, other.RegisterDate, "Дата регистрации");
            StringParamsIsEquel(this.GeneralDirector, other.GeneralDirector, "Генеральный директор");
            StringParamsIsEquel(this.License, other.License, "Лицензия");
            StringParamsIsEquel(this.WhoIssuedBy, other.WhoIssuedBy, "Кем выдана");
            StringParamsIsEquel(this.CorrAccountNumber, other.CorrAccountNumber, "Корреспондирующий счет");
            StringParamsIsEquel(this.Additional, other.Additional, "Дополнительно");
            StringParamsIsEquel(this.Site, other.Site, "Сайт");

            return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
            FieldsChanges.Add(fieldName);
        }

        [NotMapped]
        public List<string> FieldsChanges { get; set; } = new List<string>();

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;
    }  
}
