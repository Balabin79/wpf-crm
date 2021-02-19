using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dental.Interfaces;
using Dental.Models;

namespace Dental.Repositories
{
    class EmployeeRepository
    {
        

        public ObservableCollection<Employee> GetAll()
        {
            return new ObservableCollection<Employee>
                {
                   /* new Employee() {FirstName="Александр", LastName="Иванов", MiddleName="Иванович", BirthDate = new DateTime(1980, 7, 20),
                        Email = "ivanov@ya.ru", Skype = "gfregrgg", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Анатолий", LastName="Парфенов", MiddleName="Сергеевич", BirthDate = new DateTime(1972, 12, 01),
                        Email = "fdsfdf45@ya.ru", Skype = "rgrgrg", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Сергей", LastName="Пивоваров", MiddleName="Алексеевич", BirthDate = new DateTime(1977, 04, 05),
                        Email = "pivovarov@ya.ru", Skype = "grgrgrgr", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Светлана", LastName="Светлакова", MiddleName="Сергеевна", BirthDate = new DateTime(1983, 06, 06),
                        Email = "dwdwdww@ya.ru", Skype = "rgrgrgrgr", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Марина", LastName="Кравитц", MiddleName="Андреевна", BirthDate = new DateTime(1997, 10, 10),
                        Email = "dwdwdwdw@ya.ru", Skype = "rgrgrg", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Игорь", LastName="Сечин", MiddleName="Иванович", BirthDate = new DateTime(1992, 02, 20),
                        Email = "", Skype = "fwewwfwfw", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Виталий", LastName="Милонов", MiddleName="Сергеевич", BirthDate = new DateTime(1992, 12, 20),
                        Email = "262grgrgrg@ya.ru", Skype = "wefwfefef", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Семен", LastName="Слепаков", MiddleName="Семенович", BirthDate = new DateTime(1987, 12, 20),
                        Email = "fd2f662dff@ya.ru", Skype = "fwefwefwef", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Александр", LastName="Разенбаум", MiddleName="Александрович", BirthDate = new DateTime(1982, 12, 20),
                        Email = "fefefefeff@ya.ru", Skype = "wqefefwefwfwe", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    },

                    new Employee() {FirstName="Андрей", LastName="Губин", MiddleName="Тимофеевич", BirthDate = new DateTime(1977, 12, 20),
                        Email = "dfwewwww@ya.ru", Skype = "dawwd", Address = "", MobilePhone = "", HomePhone = "",
                        Status = new EmployeeStatus(), HireDate = new DateTime(2019, 7, 20), DismissalDate = new DateTime(),
                        Inn = "1234567890", Organizations = new List<Organization>(), Specialities = new List<Speciality>(),
                        Roles = new List<Role>(), Login = "", Password = ""
                    }*/

                };
        }
    }
}
