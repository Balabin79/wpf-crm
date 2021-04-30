using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dental.Models
{
    [Table("Employes")]
    class Employee : User, System.ComponentModel.IDataErrorInfo
    {
        // Контактная информация
        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email { get; set; }

        public string Skype { get; set; }

        public int? AddressId { get; set; }
        public  Address Address { get; set; }

        [Display(Name = "Моб.телефон")]
        [Phone(ErrorMessage = @"В поле ""Мобильный телефон"" введено некорректное значение")]
        [Required(ErrorMessage = @"Поле ""Мобильный телефон"" обязательно для заполнения")]
        public string MobilePhone { get; set; }

        [Display(Name = "Дом.телефон")]
        [Phone(ErrorMessage = @"В поле ""Домашний телефон"" введено некорректное значение")]
        public string HomePhone { get; set; }

        public int? EmployeeStatusId { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; } // статус (работает, уволен и т.д.)

        [Display(Name = "Дата приема")]
        public string HireDate { get; set; } = DateTime.Now.ToShortDateString().ToString(); // дата приема на работу

        [Display(Name = "Дата увольнения")]
        public string DismissalDate { get; set; } = DateTime.Now.ToShortDateString().ToString(); // дата увольнения

        [Display(Name = "ИНН")]
        [MaxLength(12, ErrorMessage = @"Длина строки в поле ""ИНН"" не более 12 цифр")]
        [MinLength(12, ErrorMessage = @"Длина строки в поле ""ИНН"" не менее 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Inn { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [NotMapped]
        public string SpecialitiesName
        {
            get => EmployesSpecialities.Count < 1 ? "": String.Join(", ", EmployesSpecialities.Select(d => d.Speciality?.Name).ToList());
        }

        [NotMapped]
        public string OrganizationsName
        {
            get => EmployesInOrganizations.Count < 1 ? "" : String.Join(", ", EmployesInOrganizations.Select(d => d.Organization?.Name).ToList());
        }

        public List<EmployesInOrganizations> EmployesInOrganizations { get; set; }
        public List<EmployesSpecialities> EmployesSpecialities { get; set; }


        public bool this[PropertyInfo prop, Employee item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Photo": return item.Photo == Photo;
                    case "FirstName": return item.FirstName == FirstName;
                    case "LastName": return item.LastName == LastName;
                    case "MiddleName": return item.MiddleName == MiddleName;
                    case "BirthDate": return item.BirthDate == BirthDate;
                   // case "GenderId": return item.GenderId == GenderId;
                    case "Inn": return item.Inn == Inn;
                    case "Email": return item.Email == Email;
                    case "Skype": return item.Skype == Skype;
                    case "Address": return item.Address == Address;
                    case "MobilePhone": return item.MobilePhone == MobilePhone;
                    case "HomePhone": return item.HomePhone == HomePhone;
                    case "Status": return item.EmployeeStatus == EmployeeStatus;
                    case "EmployeeStatusId": return item.EmployeeStatusId == EmployeeStatusId;
                    case "HireDate": return item.HireDate == HireDate;
                    case "DismissalDate": return item.DismissalDate == DismissalDate;
                    case "Login": return item.Login == Login;
                    case "Password": return item.Password == Password;
                    default: return true;
                }
            }
        }

        public void Copy(Employee copy)
        {
             Id = copy.Id;
             Photo = copy.Photo;
             FirstName = copy.FirstName;
             LastName = copy.LastName;
             MiddleName = copy.MiddleName;
             BirthDate = copy.BirthDate;
           //  GenderId = copy.GenderId;
             Inn = copy.Inn;
             Email = copy.Email;
             Skype = copy.Skype;
             Address = copy.Address;
             MobilePhone = copy.MobilePhone;
             HomePhone = copy.HomePhone;
             EmployeeStatus = copy.EmployeeStatus;
             EmployeeStatusId = copy.EmployeeStatusId;
             HireDate = copy.HireDate;
             DismissalDate = copy.DismissalDate;
             Login = copy.Login;
             Password = copy.Password;
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                return IDataErrorInfoHelper.GetErrorText(this, columnName);
            }
        }


    }
}
