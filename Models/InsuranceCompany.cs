using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("InsuranceCompanies")]
    class InsuranceCompany : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name { get; set; } = "Новая cтраховая компания";

        [MaxLength(255)]
        [Display(Name = "Код")]
        public string Code { get; set; } = "";

        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address { get; set; } = "";

        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; } = "";

        [Display(Name = "Лицензия")]
        public string License { get; set; } = "";

        [Display(Name = "Дата выдачи")]
        public string StartDateLicense { get; set; } = DateTime.Now.ToShortDateString().ToString();

        [Display(Name = "Дата окончания")]
        public string EndDateLicense { get; set; } = DateTime.Now.ToShortDateString().ToString();

        public Phone Phone { get; set; }
        public Phone AdditionalPhone { get; set; }
        public Phone Fax { get; set; }
        public Emails Email { get; set; }


        public bool this[PropertyInfo prop, InsuranceCompany item]
        {
            get
            {
                switch(prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name" : return item.Name == Name;
                    case "Code": return item.Code == Code;
                    case "Address": return item.Address == Address;
                    case "LegalAddress": return item.LegalAddress == LegalAddress;
                    case "Phone": return item.Phone == Phone;
                    case "AdditionalPhone": return item.AdditionalPhone == AdditionalPhone;
                    case "Fax": return item.Fax == Fax;
                    case "Email": return item.Email == Email;

                    case "License": return item.License == License;
                    case "StartDateLicense": return item.StartDateLicense == StartDateLicense;
                    case "EndDateLicense": return item.EndDateLicense == EndDateLicense;
                    default: return true;
                }
            }         
        }

        public void Copy(InsuranceCompany copy)
        {
            Name = copy.Name;
            Code = copy.Code;
            Address = copy.Address;
            LegalAddress = copy.LegalAddress;
            Phone = copy.Phone;
            AdditionalPhone = copy.AdditionalPhone;
            Fax = copy.Fax;
            Email = copy.Email;            
            License = copy.License;
            StartDateLicense = copy.StartDateLicense;
            EndDateLicense = copy.EndDateLicense;
        }
    }
}
