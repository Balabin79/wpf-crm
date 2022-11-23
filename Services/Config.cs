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

        public static string PathToEmployeesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "B6Dental", "Employees");

        public static string PathToOrgDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "B6Dental", "Organization");

        public static string PathToClientsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "B6Dental", "Clients");

        public static string PathToClientsPhotoDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "B6Dental", "Clients", "Photo");

        public static string PathToFilesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "B6Dental", "Files");
    }
}
