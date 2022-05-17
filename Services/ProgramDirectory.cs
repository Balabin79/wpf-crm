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
        private static string GetPathToProgrammDirectory() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);
                       
        public static string GetPathToPatientsCardsDirectoty() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);      

        private static string GetPathToEmployeesCardsDirectoty() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), EMPLOYEES_CARDS_DIRECTORY);
        
        public static string GetPathIdsDirectoty() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), IDS_DIRECTORY);
        
        private static string GetPathOrgDirectoty() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ORG_DIRECTORY);
        
                       
        public static string GetPathMyDocuments() => Environment.GetFolderPath(Environment.SpecialFolder.Personal);       
        public static DirectoryInfo GetPatientCardDirectory(string patientCardNumber) => new DirectoryInfo(Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber));
        
        public static DirectoryInfo GetEmployeeCardDirectory(string employeeCardNumber) => new DirectoryInfo(Path.Combine(GetPathToEmployeesCardsDirectoty(), employeeCardNumber));
       
        /** Get files from directories **/

        public static IEnumerable<FileInfo> GetFilesFromPatientCardDirectory(string patientCardNumber) => GetFilesFromDirectory(Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber));
        
        public static IEnumerable<FileInfo> GetFilesFromOrgDirectory() => GetFilesFromDirectory(GetPathOrgDirectoty());
        
        
        private static IEnumerable<FileInfo> GetFilesFromDirectory(string path) => new DirectoryInfo(path).GetFiles();


        public static ObservableCollection<FileInfo> GetIds() 
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
            foreach (var filePath in filesNames)  Ids.Add(new FileInfo(filePath));
            return Ids;
        }
       
        /** Has directories **/

        public static bool HasMainProgrammDirectory() => Directory.Exists(GetPathToProgrammDirectory());       
        public static bool HasPatientsCardsDirectoty() => Directory.Exists(GetPathToPatientsCardsDirectoty());
        
        public static bool HasPatientCardDirectory(string patientCardNumber) => Directory.Exists(Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber));
        
        public static bool HasIdsDirectoty() => Directory.Exists(GetPathIdsDirectoty());      
        public static bool HasOrgDirectoty() => Directory.Exists(GetPathOrgDirectoty());       
        
       
        /** Create directories **/

        public static DirectoryInfo CreateMainProgrammDirectoryForPatientCards() => (HasPatientsCardsDirectoty()) ?
            new DirectoryInfo(GetPathToPatientsCardsDirectoty()) :
            Directory.CreateDirectory(GetPathToPatientsCardsDirectoty());
        

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

        /** Remove directories **/

        public static void RemoveFileFromPatientsCard(string patientCardNumber, FileInfo file) => File.Delete(Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, (file.FullName)));
                           
        
        public static void RemoveFileFromOrgDirectory(FileInfo file) => file.Delete();          
        
        public static void RemoveIDSFile(FileInfo file) => file.Delete();
        
        public static void RemoveAllOrgFiles()
        {
            var files = GetFilesFromOrgDirectory();
            foreach (var file in files) RemoveFileFromOrgDirectory(file);
        }

        /** Save files in directories **/

        public static void SaveInPatientCardDirectory(string patientCardNumber, FileInfo file) => File.Copy(file.FullName, Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, (file.FullName)), true);
        

        public static void SaveInOrgDirectory(FileInfo file) =>  File.Copy(file.FullName, Path.Combine(GetPathOrgDirectoty(), (file.FullName)), true);


        public static bool FileExistsInPatientCardDirectory(string patientCardNumber, string fileName)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, fileName);
            return new FileInfo(path).Exists;
        }
        
        public static void ImportIds(FileInfo file)
        {
            var newPath = Path.Combine(GetPathOrgDirectoty(), (file.FullName));
            File.Copy(file.FullName, newPath, true);
        }
        

        public static List<string> Errors { get; set; } = new List<string>();
        public static bool HasErrors() => Errors.Count > 0;
    }
}
