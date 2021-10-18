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
        public const string IDS_DIRECTORY = "Dental\\Ids";

        private static string GetPathToProgrammDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);
        }        
        
        private static string GetPathToPatientsCardsDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PATIENTS_CARDS_DIRECTORY);
        }

        private static string GetPathIdsDirectoty()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), IDS_DIRECTORY);
        }

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

        public static DirectoryInfo CreateMainProgrammDirectoryForPatientCards()
        {
            if (HasPatientsCardsDirectoty()) return new DirectoryInfo(GetPathToPatientsCardsDirectoty());
            return Directory.CreateDirectory(GetPathToPatientsCardsDirectoty());
        }

        public static bool FileExistsInPatientCardDirectory(string patientCardNumber, string fileName)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, fileName);
            return new FileInfo(path).Exists;
        }

        public static DirectoryInfo CreatePatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);
            if (HasPatientCardDirectory(patientCardNumber)) return new DirectoryInfo(path);
            return Directory.CreateDirectory(path);
        }
       
        public static DirectoryInfo GetPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);
            return new DirectoryInfo(path);
        }

        public static IEnumerable<ClientFiles> GetFilesFromPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber);          
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            List<ClientFiles> clientFiles = new List<ClientFiles>();

            foreach ( var i in files)
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
      
        public static ObservableCollection<FileInfo> GetIdsFilesNames()
        {
            ObservableCollection<FileInfo> Ids = new ObservableCollection<FileInfo>();
            var path = GetPathIdsDirectoty();
            IEnumerable<string> filesNames = Directory.EnumerateFiles(path).ToList();
            foreach (var filePath in filesNames)
            {
                Ids.Add(new FileInfo(filePath));
            }
            return Ids;
        }
        /*
        public static void ImportIds(FileInfo file)
        {

        }
        */
        public static void SaveInPatientCardDirectory(string patientCardNumber, ClientFiles file)
        {
             var newPath = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, (file.FullName));
            File.Copy(file.Path, newPath, true);
            file.Path = newPath;

        }

        public static void RemoveFileFromPatientsCard(string patientCardNumber, ClientFiles file)
        {
            var path = Path.Combine(GetPathToPatientsCardsDirectoty(), patientCardNumber, (file.FullName));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static List<string> Errors { get; set; } = new List<string>();
        public static bool HasErrors() => Errors.Count > 0;
    }
}
