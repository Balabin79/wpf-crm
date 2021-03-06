using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("Employes")]
    class Employee : User
    {
        // Контактная информация
        [EmailAddress]
        public string Email { get; set; } = "example@company.com";

        public string Skype { get; set; } = "";

        [Display(Name = "Адрес")]
        public string Address { get; set; } = "";

        [Display(Name = "Моб.телефон")]
        [Phone]
        [Required]
        public string MobilePhone { get; set; } = "9111111111";

        [Display(Name = "Дом.телефон")]
        [Phone]
        public string HomePhone { get; set; } = "9111111111";

        public int EmployeeStatusId { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; } // статус (работает, уволен и т.д.)

        [Display(Name = "Дата приема")]
        public string HireDate { get; set; } = DateTime.Now.ToShortDateString().ToString(); // дата приема на работу

        [Display(Name = "Дата увольнения")]
        public string DismissalDate { get; set; } = DateTime.Now.ToShortDateString().ToString(); // дата увольнения

        [Display(Name = "ИНН")]
        public string Inn { get; set; } = "";

        [Display(Name = "Логин")]
        public string Login { get; set; } = "";

        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";


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
                    case "Gender": return item.Gender == Gender;
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
             Gender = copy.Gender;
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
    }
}
