using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services
{
    public static class Config
    {

        public static string PathToEmployeesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Employees");

        public static string PathToDataTablesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "B6Dental", "Data");
    }
}
