using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using DevExpress.Mvvm.Native;
using Microsoft.EntityFrameworkCore;
using B6CRM.Models;
using B6CRM.Models.Base;

namespace B6CRM.Services
{
    public class Config
    {
        public Config()
        {
            try
            {
                DBName = defaultDBName;
                ConnectionString = Path.Combine(defaultPath, DBName);
                PathToProgram = defaultPath;

                // открываем файл и пытаемся прочесть содержимое json
                if (File.Exists(PathToConfig))
                {
                    var json = File.ReadAllText(PathToConfig).Trim();
                    if (json.Length > 10 && JsonSerializer.Deserialize(json, new StoreConnectToDb().GetType()) is StoreConnectToDb config)
                    {
                        ConnectionString = config.ConnectionString;
                        DbType = config.Db;
                        PathToProgram = defaultPath;
                        if (DbType == 1)
                        {
                            PathToProgram = Path.Combine("\\\\", config.PostgresConnect?.Host, "B6 Software", "B6 CRM");

                            Host = config.PostgresConnect?.Host;
                            Port = config.PostgresConnect?.Port;
                            DatabaseName = config.PostgresConnect?.Database;
                            UserName = config.PostgresConnect?.Username;
                            Password = config.PostgresConnect?.Password;
                        }
                    }
                }

                PathToEmployeesDirectory = Path.Combine(PathToProgram, "Employees");
                PathToOrgDirectory = Path.Combine(PathToProgram, "Organization");
                //PathToClientsDirectory = Path.Combine(PathToProgram, "Clients");
                PathToDocumentsDirectory = Path.Combine(PathToProgram, "Documents");
                PathToFilesDirectory = Path.Combine(PathToProgram, "Files");
                PathToProgramDirectory = PathToProgram;
            }
            catch (Exception e)
            {
                DBName = defaultDBName;
                ConnectionString = Path.Combine(defaultPath, DBName);
                PathToProgram = defaultPath;
                DbType = (int)DbList.SQLite;
                Log.ErrorHandler(e);
            }
        }

        public string ConnectionString;
        public string DBName;
        public string PathToProgram;
        public int DbType;

        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }

        // эти пути всегда статичны       
        public static string defaultPath = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)).FullName, "B6 Software", "B6 CRM");
        public static string defaultDBName = "B6Crm.db";
        public static string PathToDbDefault = Path.Combine(defaultPath, defaultDBName);
        public static string PathToConfig = Path.Combine(defaultPath, "B6Crm.conf");


        // эти пути задаются динамически
        public string PathToEmployeesDirectory;
        public string PathToOrgDirectory;
        // public string PathToClientsDirectory;
        public string PathToDocumentsDirectory;
        public string PathToFilesDirectory;
        public string PathToProgramDirectory;

        public string GetPathToLogo()
        {
            var logo = "";
            var files = new string[] { };
            if (Directory.Exists(PathToOrgDirectory)) files = Directory.GetFiles(PathToOrgDirectory);

            foreach (var i in files) if (i.Contains("Logo")) logo = i;

            return Path.Combine(PathToOrgDirectory, logo);
        }

        public enum DbList { SQLite = 0, PostgreSQL = 1 }
    }
}
