using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
/*using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Common;
using DevExpress.DevAV;*/
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    class Employee //: IDataErrorInfo
    {

        public static Employee Create()
        {
            return ViewModelSource.Create(() => new Employee());
        }
        //public Employee();

        [Key]
        public long Id { get; set; }

        // Общая информация
        [NotMapped]
        public Image Photo { get; set; }
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }

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


        public string Status { get; set; } // статус (работает, уволен и т.д.)
        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; } // дата приема на работу
        public DateTime? DismissalDate { get; set; } // дата увольнения
        public string Inn { get; set; }
        public ICollection<Organization> Organizations { get; set; }  // Организации (в которых работает)
        public ICollection<Speciality> Specialities { get; set; }
        public string EmployeeStatus { get; set; }
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





    }
}
