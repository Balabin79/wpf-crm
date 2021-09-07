using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dental.Models.Base;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Employes")]
    class Employee : AbstractBaseModel, IDataErrorInfo
    {
        // Общая информация
        [Display(Name = "Фото")]
        public string Photo { get; set; }

        [NotMapped]
        public ImageSource Image { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Фамилия"" не более 255 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; }

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? FirstName + " " + LastName : FirstName + " " + MiddleName + " " + LastName;
        }
        // Контактная информация
        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email { get; set; }

        public string Skype { get; set; }

        [Display(Name = "Моб.телефон")]
        [Phone(ErrorMessage = @"В поле ""Мобильный телефон"" введено некорректное значение")]
        [Required(ErrorMessage = @"Поле ""Мобильный телефон"" обязательно для заполнения")]
        public string MobilePhone { get; set; }

        [Display(Name = "Дом.телефон")]
        [Phone(ErrorMessage = @"В поле ""Домашний телефон"" введено некорректное значение")]
        public string HomePhone { get; set; }

        [Display(Name = "Дата приема")]
        public string HireDate { get; set; } // дата приема на работу

        [Display(Name = "Уволен")]
        public int? IsDismissed { get; set; }

        [Display(Name = "Дата увольнения")]
        public string DismissalDate { get; set; } //= DateTime.Now.ToShortDateString().ToString(); // дата увольнения

        [Display(Name = "ИНН")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = @"Формат ""ИНН""- 12 цифр")]
        public string Inn { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        /*
        [NotMapped]
        public string SpecialitiesName
        {
            get => EmployesSpecialities.Count < 1 ? "": String.Join(", ", EmployesSpecialities.Select(d => d.Speciality?.Name).ToList());
        }

        public List<EmployesSpecialities> EmployesSpecialities { get; set; }
        */

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
                    case "MobilePhone": return item.MobilePhone == MobilePhone;
                    case "HomePhone": return item.HomePhone == HomePhone;
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
             MobilePhone = copy.MobilePhone;
             HomePhone = copy.HomePhone;
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
