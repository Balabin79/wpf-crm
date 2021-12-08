using Dental.Infrastructures.Logs;
using Dental.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services
{
    static class ProgramDirectory
    {
        public const string ALLUSERSPROFILE = "@%ALLUSERSPROFILE%";  // C:\ProgramData 
        public const string APPDATA = "@%APPDATA%";   // C:\Users\Username\AppData\Roaming
        public const string COMMONPROGRAMFILES = "@%COMMONPROGRAMFILES%";    // C:\Program Files\Common Files
        public const string COMMONPROGRAMFILES86 = "@%COMMONPROGRAMFILES(x86)%";   // C:\Program Files(x86)\Common Files
        public const string COMSPEC = "@%COMSPEC%";   // C:\Windows\System32\cmd.exe
        public const string HOMEDRIVE  = "@%HOMEDRIVE%"; // C:
        public const string HOMEPATH ="@%HOMEPATH%";  // C:\Users\Username
        public const string LOCALAPPDATA = "@%LOCALAPPDATA%"; // C:\Users\Username\AppData\Local
        public const string PROGRAMDATA = "@%PROGRAMDATA%";   // C:\ProgramData
        public const string PROGRAMFILES = "@%PROGRAMFILES%";  // C:\Program Files
        public const string PROGRAMFILES86 = "@%PROGRAMFILES(X86)%";  // C:\Program Files(x86) (only in 64-bit version)
        public const string PUBLIC = "@%PUBLIC%";    // C:\Users\Public
        public const string SystemDrive = "@%SystemDrive%";  // C:
        public const string SystemRoot ="@%SystemRoot%";   // C:\Windows
        public const string TEMP = "@%TEMP% and %TMP%";  // C:\Users\Username\AppData\Local\Temp
        public const string USERPROFILE = "@%USERPROFILE%";   // C:\Users\Username
        public const string WINDIR = "@%WINDIR%";   // C:\Windows
        public const string PROGRAMM_NAME = "Dental";
        public const string PATIENTS_CARDS_DIRECTORY = "Dental\\PatientsCards";
        public const string EMPLOYEES_CARDS_DIRECTORY = "Dental\\EmployeesCards";
        public const string IDS_DIRECTORY = "Dental\\Ids";
        public const string ORG_DIRECTORY = "Dental\\Organization";
        public const string LOGO_DIRECTORY = "Dental\\Logo";

        
        /** Get path to directories **/
        private static string GetPathToProgrammDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);
        }        
        
        private static string GetPathToPatientsCardsDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);
        }
        
        private static string GetPathToEmployeesCardsDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), EMPLOYEES_CARDS_DIRECTORY);
        }

        public static string GetPathIdsDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), IDS_DIRECTORY);
        }

        private static string GetPathOrgDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ORG_DIRECTORY);
        }

        public static string GetPathLogoDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LOGO_DIRECTORY);
        }        
        
        public static string GetPathMyDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public static DirectoryInfo GetPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);
            return new DirectoryInfo(path);
        }

        public static DirectoryInfo GetEmployeeCardDirectory(string employeeCardNumber)
        {
            string path = Path.Combine(GetPathToEmployeesCardsDirectoty(), employeeCardNumber);
            return new DirectoryInfo(path);
        }

        /** Get files from directories **/

        public static IEnumerable<ClientFiles> GetFilesFromPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);
            return GetFilesFromDirectory(path);
        }

        public static IEnumerable<ClientFiles> GetFilesFromOrgDirectory()
        {
            string path = GetPathOrgDirectoty();
            return GetFilesFromDirectory(path);
        }

        public static IEnumerable<ClientFiles> GetFilesFromLogoDirectory()
        {
            string path = GetPathLogoDirectoty();
            return GetFilesFromDirectory(path);
        }

        private static IEnumerable<ClientFiles> GetFilesFromDirectory(string path)
        {
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            List<ClientFiles> clientFiles = new List<ClientFiles>();

            foreach (var i in files)
            {
                ClientFiles cf = new ClientFiles();
                cf.Path = i.FullName;
                cf.Name = Path.GetFileNameWithoutExtension(i.FullName);
                cf.FullName = Path.GetFileName(i.FullName);
                cf.Size = i.Length.ToString();
                cf.DateCreated = i.CreationTime.ToShortDateString();
                cf.Extension = i.Extension;
                cf.Status = ClientFiles.STATUS_SAVE_RUS;

                clientFiles.Add(cf);
            }
            return clientFiles;
        }

        public static ObservableCollection<FileInfo> GetIds() //////
        {
            ObservableCollection<FileInfo> Ids = new ObservableCollection<FileInfo>();
            var path = GetPathIdsDirectoty();

            IEnumerable<string> filesNames = new List<string>();
            string[] formats = new string[] { "*.docx", "*.doc", "*.rtf", "*.odt", "*.epub", "*.txt", "*.html", "*.htm", "*.mht", "*.xml" };
            foreach (var format in formats)
            {
                var collection = Directory.EnumerateFiles(path, format).ToList();
                if (collection.Count > 0) filesNames = filesNames.Union(collection);
            }

            foreach (var filePath in filesNames)
            {
                Ids.Add(new FileInfo(filePath));
            }
            return Ids;
        }
       
        /** Has directories **/

        public static bool HasMainProgrammDirectory()
        {
            return Directory.Exists(GetPathToProgrammDirectory());
        }

        public static bool HasPatientsCardsDirectoty()
        {
            return Directory.Exists(GetPathToPatientsCardsDirectoty());
        }

        public static bool HasPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);
            return Directory.Exists(path);
        }

        public static bool HasIdsDirectoty()
        {
            return Directory.Exists(GetPathIdsDirectoty());
        }

        public static bool HasOrgDirectoty()
        {
            return Directory.Exists(GetPathOrgDirectoty());
        }

        public static bool HasLogoDirectoty()
        {
            return Directory.Exists(GetPathLogoDirectoty());
        }

       
        /** Create directories **/

        public static DirectoryInfo CreateMainProgrammDirectoryForPatientCards()
        {
            if (HasPatientsCardsDirectoty()) return new DirectoryInfo(GetPathToPatientsCardsDirectoty());
            return Directory.CreateDirectory(GetPathToPatientsCardsDirectoty());
        }

        public static DirectoryInfo CreatePatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);
            if (HasPatientCardDirectory(patientCardNumber)) return new DirectoryInfo(path);
            return Directory.CreateDirectory(path);
        }

        public static DirectoryInfo CreateOrgDirectory()
        { 
            string path = GetPathOrgDirectoty();
            if (HasOrgDirectoty()) return new DirectoryInfo(path);
            return Directory.CreateDirectory(path);
        }

        public static DirectoryInfo CreateLogoDirectory()
        {
            string path = GetPathLogoDirectoty();
            if (HasLogoDirectoty()) return new DirectoryInfo(path);
            return Directory.CreateDirectory(path);
        }

        /** Remove directories **/

        public static void RemoveFileFromPatientsCard(string patientCardNumber, ClientFiles file)
        {
            var path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, (file.FullName));
            if (File.Exists(path))
            {
                File.Delete(path);
            }                
        }

        public static void RemoveFileFromOrgDirectory(ClientFiles file)
        {
            var path = Path.Combine(GetPathOrgDirectoty(), (file.FullName));
            if (File.Exists(path)) File.Delete(path);          
        }

        public static void RemoveIDSFile(FileInfo file)
        {
            var path = Path.Combine(GetPathIdsDirectoty(), (file.FullName));
            if (File.Exists(path)) File.Delete(path);
        }

        public static void RemoveAllOrgFiles()
        {
            var files = GetFilesFromOrgDirectory();
            foreach (var file in files) RemoveFileFromOrgDirectory(file);
        }

        public static void RemoveLogoFile()
        {
            try
            {
                FileInfo[] files = new DirectoryInfo(GetPathLogoDirectoty()).GetFiles();
                foreach (var file in files)
                {
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                }
            }
            catch (Exception e)
            {
                (new ViewModelLog(e)).run();
            }



        }

        /** Save files in directories **/

        public static void SaveInPatientCardDirectory(string patientCardNumber, ClientFiles file)
        {
            var newPath = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, (file.FullName));
            File.Copy(file.Path, newPath, true);
            file.Path = newPath;
        }

        public static void SaveInOrgDirectory(ClientFiles file)
        {
            var newPath = Path.Combine(GetPathOrgDirectoty(), (file.FullName));
            File.Copy(file.Path, newPath, true);
            file.Path = newPath;
        }

        public static void SaveFileInLogoDirectory(ClientFiles file)
        {

            RemoveLogoFile();
            string newPath = Path.Combine(GetPathLogoDirectoty(), (file.Name));
            File.Copy(file.Path, newPath, true);
            file.Path = newPath;     
        }

        /****/

        public static bool FileExistsInPatientCardDirectory(string patientCardNumber, string fileName)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, fileName);
            return new FileInfo(path).Exists;
        }
        /*
        public static void ImportIds(FileInfo file)
        {

        }
        */

        public static List<string> Errors { get; set; } = new List<string>();
        public static bool HasErrors() => Errors.Count > 0;
    }
}
