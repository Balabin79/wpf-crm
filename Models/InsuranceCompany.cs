using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System;

namespace Dental.Models
{
    [Table("InsuranceCompanies")]
    class InsuranceCompany : AbstractBaseModel, IDataErrorInfo, ITreeModel, ITreeViewCollection
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

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

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = "89111111111";

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Дополнительный телефон")]
        public string AdditionalPhone { get; set; } = "88001000000";

        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [Phone]
        [Display(Name = "Факс")]
        public string Fax { get; set; } = "89111111111";

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "example@company.com";

        public int? ParentId { get; set; }
        public int IsDir { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


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
