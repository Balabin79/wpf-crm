using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Dental.Models
{
    [Table("Org")]
    public class Org : AbstractBaseModel, ICloneable
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Address { get; set; }
        public string LegalAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Bik { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string Ogrn { get; set; }
        public string LicenseDate { get; set; }
        public string GeneralDirector { get; set; }
        public string LicenseName { get; set; }
        public string WhoIssuedBy { get; set; }
        public string Additional { get; set; }
        public string Okved { get; set; }
        public string Okpo { get; set; }
        public string CorrAccountNumber { get; set; }
        public string Site { get; set; }

        public object Clone() => this.MemberwiseClone();

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

        public override string ToString() => ShortName;
    }
}
