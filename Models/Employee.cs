using DevExpress.Mvvm.POCO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Dental.Repositories;
/*using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Common;
using DevExpress.DevAV;*/

namespace Dental.Models
{
    class Employee : User 
    {
        // Контактная информация
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Address { get; set; }
        [Display(Name = "Mobile Phone")]
        [Phone]
        [Required]
        public string MobilePhone { get; set; }
        [Display(Name = "Home Phone")]
        [Phone]
        public string HomePhone { get; set; }


        public EmployeeStatus Status { get; set; } // статус (работает, уволен и т.д.)
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; } // дата приема на работу
        public DateTime? DismissalDate { get; set; } // дата увольнения
        public string Inn { get; set; }
        public ICollection<Organization> Organizations { get; set; }  // Организации (в которых работает)
        public ICollection<Speciality> Specialities { get; set; }
        public ICollection<Role> Roles { get; set; }

        public string Login { get; set; }
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
                    listEmployes = EmployeeRepository.GetFakeListEmployee();
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
