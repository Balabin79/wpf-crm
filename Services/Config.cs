using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

namespace Dental.Services
{
    public static class Config
    {
        static Config()
        {
            try
            {
                DBName = defaultDBName;
                ConnectionString = Path.Combine(defaultPath, DBName);
                PathToProgram = defaultPath;             

                // открываем файл и пытаемся прочесть содержимое json
                if (File.Exists("./dental.conf")) 
                {
                    var json = File.ReadAllText("./dental.conf").Trim();

                    if (json.Length > 10 && JsonSerializer.Deserialize(json, new UserConfig().GetType()) is UserConfig config)
                    {
                        DBName = config.DBName ?? defaultDBName;
                        ConnectionString = config.ConnectionString ?? Path.Combine(defaultPath, DBName);
                        PathToProgram = config.PathToProgram ?? defaultPath;
                    }
                }
                PathToEmployeesDirectory = Path.Combine(PathToProgram, "Employees");
            }
            catch
            {
                DBName = defaultDBName;
                ConnectionString = Path.Combine(defaultPath, DBName);
                PathToProgram = defaultPath;
            }
        }
        public static string ConnectionString;
        public static string DBName;
        public static string PathToProgram;

        // This should give you something like C:\Users\Public        

        private static string defaultPath = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)).FullName, "B6 Software", "Dental");

        public static string defaultDBName = "dental.db";

        public static string PathToDbDefault = Path.Combine(defaultPath, defaultDBName);

        public static string PathToEmployeesDirectory;

        public static string PathToOrgDirectory = Path.Combine(defaultPath, "Organization");

        public static string PathToClientsDirectory = Path.Combine(defaultPath, "Clients");

        public static string PathToClientsPhotoDirectory = Path.Combine(defaultPath, "Clients", "Photo");

        public static string PathToClientsDocumentsDirectory = Path.Combine(defaultPath, "Clients", "Documents");

        public static string PathToFilesDirectory = Path.Combine(defaultPath, "Files");
        
        public static string PathToProgramDirectory = Path.Combine(defaultPath);

        public static string GetPathToLogo() => Path.Combine(PathToOrgDirectory, "Logo.jpg");      
    }
}
