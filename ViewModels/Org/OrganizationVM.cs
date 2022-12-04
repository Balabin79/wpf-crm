using Dental.Models;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.ViewModels.Org
{
    public class OrganizationVM : ViewModelBase, IDataErrorInfo
    {
        public int? Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

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

        public void Copy(Organization model)
        {
            Id = model.Id;
            Name = model.Name;
            ShortName = model.ShortName;
            Kpp = model.Kpp;
            Inn = model.Inn;
            Address = model.Address;
            Phone = model.Phone;
            Email = model.Email;
            AccountNumber = model.AccountNumber;
            CorrAccountNumber = model.CorrAccountNumber;
            Bik = model.Bik;
            BankName = model.BankName;
            Ogrn = model.Ogrn;
            LicenseDate = model.LicenseDate;
            GeneralDirector = model.GeneralDirector;
            LicenseName = model.LicenseName;
            Site = model.Site;
            WhoIssuedBy = model.WhoIssuedBy;
            Okpo = model.Okpo;
        }

        public bool HasChanges() 
        {
            using (var db = new ApplicationContext())
            {
                var model = db.Organizations.FirstOrDefault();
                return
                    Name != model?.Name ||
                    ShortName != model?.ShortName ||
                    Kpp != model?.Kpp ||
                    Inn != model?.Inn ||
                    Address != model?.Address ||
                    Phone != model?.Phone ||
                    Email != model?.Email ||
                    AccountNumber != model?.AccountNumber ||
                    CorrAccountNumber != model?.CorrAccountNumber ||
                    Bik != model?.Bik ||
                    BankName != model?.BankName ||
                    Ogrn != model?.Ogrn ||
                    LicenseDate != model?.LicenseDate ||
                    GeneralDirector != model?.GeneralDirector ||
                    LicenseName != model?.LicenseName ||
                    Site != model?.Site ||
                    WhoIssuedBy != model?.WhoIssuedBy ||
                    Okpo != model?.Okpo;
            }
        }
    }
}
