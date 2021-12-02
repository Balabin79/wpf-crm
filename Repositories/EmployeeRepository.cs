using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Repositories
{
    public static class EmployeeRepository
    {
        public static List<Employee> Employes { get => GetEmployes();  }

        public static List<Employee> GetEmployes() => new ApplicationContext().Employes.ToList();        
    }
}
