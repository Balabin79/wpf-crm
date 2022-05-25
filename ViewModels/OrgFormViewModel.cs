using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dental.ViewModels
{
    public class OrgFormViewModel : ViewModelBase, IDataErrorInfo
    {
        public OrgFormViewModel(Org org)
        {
            Id = org.Id;
            Guid = org.Guid;
            Name = org.Name;
            ShortName = org.ShortName;
            Kpp = org.Kpp;
            Inn = org.Inn;
            Address = org.Address;
            LegalAddress = org.LegalAddress;
            Phone = org.Phone;
            Email = org.Email;
            AccountNumber = org.AccountNumber;
            CorrAccountNumber = org.CorrAccountNumber;
            Bik = org.Bik;
            BankName = org.BankName;
            Ogrn = org.Ogrn;
            LicenseDate = org.LicenseDate;
            GeneralDirector = org.GeneralDirector;
            LicenseName = org.LicenseName;
            Site = org.Site;
            WhoIssuedBy = org.WhoIssuedBy;
            Additional = org.Additional;
            Okpo = org.Okpo;
            Okved = org.Okved;
        }

        public int Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        public string Guid
        {
            get { return GetProperty(() => Guid); }
            set { SetProperty(() => Guid, value); }
        }
 
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Наименование"" не более 255 символов")]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value.Trim()); }
        }

        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Сокращенное наименование"" не более 255 символов")]
        public string ShortName
        {
            get { return GetProperty(() => ShortName); }
            set { SetProperty(() => ShortName, value.Trim()); }
        }     

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Inn
        {
            get { return GetProperty(() => Inn); }
            set { SetProperty(() => Inn, value.Trim()); }
        }

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Kpp
        {
            get { return GetProperty(() => Kpp); }
            set { SetProperty(() => Kpp, value.Trim()); }
        }

        [Display(Name = "Фактический адрес")]
        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value.Trim()); }
        }

        [Display(Name = "Юридический адрес")]
        public string LegalAddress
        {
            get { return GetProperty(() => LegalAddress); }
            set { SetProperty(() => LegalAddress, value.Trim()); }
        }

        [Phone]
        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value?.Trim()); }
        }

        [EmailAddress]
        [MaxLength(255)]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik
        {
            get { return GetProperty(() => Bik); }
            set { SetProperty(() => Bik, value?.Trim()); }
        }

        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber
        {
            get { return GetProperty(() => AccountNumber); }
            set { SetProperty(() => AccountNumber, value?.Trim()); }
        }

        public string BankName
        {
            get { return GetProperty(() => BankName); }
            set { SetProperty(() => BankName, value?.Trim()); }
        }

        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn
        {
            get { return GetProperty(() => Ogrn); }
            set { SetProperty(() => Ogrn, value?.Trim()); }
        }

        public string LicenseDate
        {
            get { return GetProperty(() => LicenseDate); }
            set { SetProperty(() => LicenseDate, value?.Trim()); }
        }

        public string GeneralDirector
        {
            get { return GetProperty(() => GeneralDirector); }
            set { SetProperty(() => GeneralDirector, value?.Trim()); }
        }

        public string LicenseName
        {
            get { return GetProperty(() => LicenseName); }
            set { SetProperty(() => LicenseName, value?.Trim()); }
        }

        public string WhoIssuedBy
        {
            get { return GetProperty(() => WhoIssuedBy); }
            set { SetProperty(() => WhoIssuedBy, value?.Trim()); }
        }

        public string Additional
        {
            get { return GetProperty(() => Additional); }
            set { SetProperty(() => Additional, value?.Trim()); }
        }

        [MaxLength(6, ErrorMessage = "Длина не более 6 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okved
        {
            get { return GetProperty(() => Okved); }
            set { SetProperty(() => Okved, value?.Trim()); }
        }

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okpo
        {
            get { return GetProperty(() => Okpo); }
            set { SetProperty(() => Okpo, value?.Trim()); }
        }

        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string CorrAccountNumber
        {
            get { return GetProperty(() => CorrAccountNumber); }
            set { SetProperty(() => CorrAccountNumber, value?.Trim()); }
        }

        public string Site
        {
            get { return GetProperty(() => Site); }
            set { SetProperty(() => Site, value?.Trim()); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override int GetHashCode() => Guid.GetHashCode();

        public bool Equals(Org other)
        {
            if (FieldsChanges != null) FieldsChanges = new List<string>();

            if (other is Org clone && !object.ReferenceEquals(this, clone))
            {
                StringParamsIsEquel(Name, other.Name, "Наименование");
                StringParamsIsEquel(ShortName, other.ShortName, "Сокращенное наименование");
                StringParamsIsEquel(Kpp, other.Kpp, "КПП");
                StringParamsIsEquel(Inn, other.Inn, "ИНН");
                StringParamsIsEquel(Phone, other.Phone, "Телефон");
                StringParamsIsEquel(Email, other.Email, "Email");
                StringParamsIsEquel(Address, other.Address, "Фактический адрес");
                StringParamsIsEquel(LegalAddress, other.LegalAddress, "Юридический адрес");
                StringParamsIsEquel(Bik, other.Bik, "БИК");
                StringParamsIsEquel(AccountNumber, other.AccountNumber, "Расчетный счет");
                StringParamsIsEquel(BankName, other.BankName, "Наименование банка");
                StringParamsIsEquel(Ogrn, other.Ogrn, "ОГРН");
                StringParamsIsEquel(Okpo, other.Okpo, "ОКПО");
                StringParamsIsEquel(Okved, other.Okved, "ОКВЭД");
                StringParamsIsEquel(LicenseDate, other.LicenseDate, "Дата регистрации");
                StringParamsIsEquel(GeneralDirector, other.GeneralDirector, "Генеральный директор");
                StringParamsIsEquel(LicenseName, other.LicenseName, "Лицензия");
                StringParamsIsEquel(WhoIssuedBy, other.WhoIssuedBy, "Кем выдана");
                StringParamsIsEquel(CorrAccountNumber, other.CorrAccountNumber, "Корреспондирующий счет");
                StringParamsIsEquel(Additional, other.Additional, "Дополнительно");
                StringParamsIsEquel(Site, other.Site, "Сайт");
            }
            return FieldsChanges.Count == 0;
        }

        private void StringParamsIsEquel(string param1, string param2, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            FieldsChanges.Add(fieldName);
        }

        public List<string> FieldsChanges { get; set; } = new List<string>();


        public override string ToString() => ShortName;

        public Org Copy(Org model)
        {
            model.Name = Name;
            model.ShortName = ShortName;
            model.Kpp = Kpp;
            model.Inn = Inn;
            model.Address = Address;
            model.LegalAddress = LegalAddress;
            model.Phone = Phone;
            model.Email = Email;
            model.AccountNumber = AccountNumber;
            model.CorrAccountNumber = CorrAccountNumber;
            model.Bik = Bik;
            model.BankName = BankName;
            model.Ogrn = Ogrn;
            model.LicenseDate = LicenseDate;
            model.GeneralDirector = GeneralDirector;
            model.LicenseName = LicenseName;
            model.Site = Site;
            model.WhoIssuedBy = WhoIssuedBy;
            model.Additional = Additional;
            model.Okpo = Okpo;
            model.Okved = Okved;
            return model;
        }
    }
}
