using DevExpress.Mvvm.POCO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Dental.Repositories;


namespace Dental.Models
{
    class Employee : User 
    {
        // Контактная информация
        [EmailAddress]
        public string Email { get; set; }

        public string Skype { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Моб.телефон")]
        [Phone]
        [Required]
        public string MobilePhone { get; set; }

        [Display(Name = "Дом.телефон")]
        [Phone]
        public string HomePhone { get; set; }

        [Display(Name = "Статус")]
        public EmployeeStatus Status { get; set; } // статус (работает, уволен и т.д.)

        [Display(Name = "Дата приема")]
        public DateTime? HireDate { get; set; } // дата приема на работу

        [Display(Name = "Дата увольнения")]
        public DateTime? DismissalDate { get; set; } // дата увольнения

        [Display(Name = "ИНН")]
        public string Inn { get; set; }

        [Display(Name = "Организации")]
        public ICollection<Organization> Organizations { get; set; }  // Организации (в которых работает)

        [Display(Name = "Специальности")]
        public ICollection<Speciality> Specialities { get; set; }

        [Display(Name = "Роли")]
        public ICollection<Role> Roles { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        public string Password { get; set; }


        public string ShortName
        {
            get
            {
                return LastName + " " + char.ToUpper(FirstName[0]) + "." + char.ToUpper(MiddleName[0]) + ".";
            }
        }

        public string FullName
        {
            get
            {
                return LastName + " " + FirstName + " " + MiddleName;
            }
        }


        private ObservableCollection<Employee> listEmployes;
        public ObservableCollection<Employee> ListEmployes
        {
            get
            {
                if (listEmployes == null)
                {
                    //listEmployes = EmployeeRepository.GetFakeListEmployee();
                    return listEmployes;
                }
                return listEmployes;
            }
            set
            {
                Set(ref listEmployes, value);
            }
        }



    }
}
