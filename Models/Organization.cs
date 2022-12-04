using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Organizations")]
    public class Organization : AbstractBaseModel
    {
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value?.Trim()); }
        }

        public string ShortName
        {
            get { return GetProperty(() => ShortName); }
            set { SetProperty(() => ShortName, value?.Trim()); }
        }

        public string Inn
        {
            get { return GetProperty(() => Inn); }
            set { SetProperty(() => Inn, value?.Trim()); }
        }

        public string Kpp
        {
            get { return GetProperty(() => Kpp); }
            set { SetProperty(() => Kpp, value?.Trim()); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
        }

        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value?.Trim()); }
        }

        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        public string Bik
        {
            get { return GetProperty(() => Bik); }
            set { SetProperty(() => Bik, value?.Trim()); }
        }

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

        public string Ogrn
        {
            get { return GetProperty(() => Ogrn); }
            set { SetProperty(() => Ogrn, value?.Trim()); }
        }

        public string LicenseDate
        {
            get { return GetProperty(() => LicenseDate); }
            set { SetProperty(() => LicenseDate, value); }
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

        public string Okpo
        {
            get { return GetProperty(() => Okpo); }
            set { SetProperty(() => Okpo, value?.Trim()); }
        }

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
    }
}

