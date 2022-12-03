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
        // This should give you something like C:\Users\Public
        private static string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)).FullName;

        public static string PathToEmployeesDirectory = Path.Combine(path, "B6 Software", "Dental", "Employees");

        public static string PathToOrgDirectory = Path.Combine(path, "B6 Software", "Dental", "Organization");

        public static string PathToClientsDirectory = Path.Combine(path, "B6 Software", "Dental", "Clients");

        public static string PathToClientsPhotoDirectory = Path.Combine(path, "B6 Software", "Dental", "Clients", "Photo");

        public static string PathToClientsDocumentsDirectory = Path.Combine(path, "B6 Software", "Dental", "Clients", "Documents");

        public static string PathToFilesDirectory = Path.Combine(path, "B6 Software", "Dental", "Files");
        
        public static string PathToProgramDirectory = Path.Combine(path, "B6 Software", "Dental");
    }
}
