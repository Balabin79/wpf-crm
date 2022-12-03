using Dental.Models.Base;
using Dental.Services;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value?.Trim()); }
        }

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName
        {
            get { return GetProperty(() => ShortName); }
            set { SetProperty(() => ShortName, value?.Trim()); }
        }


        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn
        {
            get { return GetProperty(() => Inn); }
            set { SetProperty(() => Inn, value?.Trim()); }
        }


        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp
        {
            get { return GetProperty(() => Kpp); }
            set { SetProperty(() => Kpp, value?.Trim()); }
        }

        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
        }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value?.Trim()); }
        }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        // Банковские реквизиты
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik
        {
            get { return GetProperty(() => Bik); }
            set { SetProperty(() => Bik, value?.Trim()); }
        }

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber
        {
            get { return GetProperty(() => AccountNumber); }
            set { SetProperty(() => AccountNumber, value?.Trim()); }
        }

        [Display(Name = "Наименование банка")]
        public string BankName
        {
            get { return GetProperty(() => BankName); }
            set { SetProperty(() => BankName, value?.Trim()); }
        }

        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn
        {
            get { return GetProperty(() => Ogrn); }
            set { SetProperty(() => Ogrn, value?.Trim()); }
        }

        [Display(Name = "Дата лицензии")]
        public string LicenseDate
        {
            get { return GetProperty(() => LicenseDate); }
            set { SetProperty(() => LicenseDate, value); }
        }

        [Display(Name = "Генеральный директор")]
        public string GeneralDirector
        {
            get { return GetProperty(() => GeneralDirector); }
            set { SetProperty(() => GeneralDirector, value?.Trim()); }
        }

        [Display(Name = "Лицензия")]
        public string LicenseName
        {
            get { return GetProperty(() => LicenseName); }
            set { SetProperty(() => LicenseName, value?.Trim()); }
        }

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy
        {
            get { return GetProperty(() => WhoIssuedBy); }
            set { SetProperty(() => WhoIssuedBy, value?.Trim()); }
        }

        [Display(Name = "ОКПО")]
        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okpo
        {
            get { return GetProperty(() => Okpo); }
            set { SetProperty(() => Okpo, value?.Trim()); }
        }

        [Display(Name = "Корреспондирующий счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string CorrAccountNumber
        {
            get { return GetProperty(() => CorrAccountNumber); }
            set { SetProperty(() => CorrAccountNumber, value?.Trim()); }
        }

        [Display(Name = "Сайт")]
        public string Site
        {
            get { return GetProperty(() => Site); }
            set { SetProperty(() => Site, value?.Trim()); }
        }

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
                Address = this.Address,
                Phone = this.Phone,
                Email = this.Email,
                AccountNumber = this.AccountNumber,
                CorrAccountNumber = this.CorrAccountNumber,
                Bik = this.Bik,
                BankName = this.BankName,
                Ogrn = this.Ogrn,
                LicenseDate = this.LicenseDate,
                GeneralDirector = this.GeneralDirector,
                LicenseName = this.LicenseName,
                Site = this.Site,
                WhoIssuedBy = this.WhoIssuedBy,
                Okpo = this.Okpo
            };

        }

        public Organization Copy(Organization model)
        {
            model.Name = this.Name;
            model.ShortName = this.ShortName;
            model.Kpp = this.Kpp;
            model.Inn = this.Inn;
            model.Address = this.Address;
            model.Phone = this.Phone;
            model.Email = this.Email;
            model.AccountNumber = this.AccountNumber;
            model.CorrAccountNumber = this.CorrAccountNumber;
            model.Bik = this.Bik;
            model.BankName = this.BankName;
            model.Ogrn = this.Ogrn;
            model.LicenseDate = this.LicenseDate;
            model.GeneralDirector = this.GeneralDirector;
            model.LicenseName = this.LicenseName;
            model.Site = this.Site;
            model.WhoIssuedBy = this.WhoIssuedBy;
            model.Okpo = this.Okpo;
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
            StringParamsIsEquel(this.Phone, other.Phone, "Телефон");
            StringParamsIsEquel(this.Email, other.Email, "Email");
            StringParamsIsEquel(this.Address, other.Address, "Фактический адрес");
            StringParamsIsEquel(this.Bik, other.Bik, "БИК");
            StringParamsIsEquel(this.AccountNumber, other.AccountNumber, "Расчетный счет");
            StringParamsIsEquel(this.BankName, other.BankName, "Наименование банка");
            StringParamsIsEquel(this.Ogrn, other.Ogrn, "ОГРН");
            StringParamsIsEquel(this.Okpo, other.Okpo, "ОКПО");
            StringParamsIsEquel(this.LicenseDate, other.LicenseDate, "Дата регистрации");
            StringParamsIsEquel(this.GeneralDirector, other.GeneralDirector, "Генеральный директор");
            StringParamsIsEquel(this.LicenseName, other.LicenseName, "Лицензия");
            StringParamsIsEquel(this.WhoIssuedBy, other.WhoIssuedBy, "Кем выдана");
            StringParamsIsEquel(this.CorrAccountNumber, other.CorrAccountNumber, "Корреспондирующий счет");
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

