using Dental.Models;
using System;
using System.Collections.Generic;
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

        public static string GetPathToProgrammDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PROGRAMM_NAME);
        }

        public static bool HasMainProgrammDirectory()
        {
            return Directory.Exists(GetPathToProgrammDirectory());
        }
        
        public static DirectoryInfo CreateMainProgrammDirectory()
        {
            if (HasMainProgrammDirectory()) return new DirectoryInfo(GetPathToProgrammDirectory());
            return Directory.CreateDirectory(GetPathToProgrammDirectory());
        }

        public static bool HasPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToProgrammDirectory(), patientCardNumber);
            return Directory.Exists(path);
        }

        public static bool FileExistsInPatientCardDirectory(string patientCardNumber, string fileName)
        {
            string path = Path.Combine(GetPathToProgrammDirectory(), patientCardNumber, fileName);
            return new FileInfo(path).Exists;
        }

        public static DirectoryInfo CreatePatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToProgrammDirectory(), patientCardNumber);
            if (HasPatientCardDirectory(patientCardNumber)) return new DirectoryInfo(path);
            return Directory.CreateDirectory(path);
        }
       
        public static DirectoryInfo GetPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToProgrammDirectory(), patientCardNumber);
            return new DirectoryInfo(path);
        }

        public static IEnumerable<ClientFiles> GetFilesFromPatientCardDirectory(string patientCardNumber)
        {
            string path = Path.Combine(GetPathToProgrammDirectory(), patientCardNumber);          
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            List<ClientFiles> clientFiles = new List<ClientFiles>();

            foreach ( var i in files)
            {
                ClientFiles cf = new ClientFiles();
                cf.Path = i.FullName;
                cf.Name = i.Name;
                cf.Size = i.Length.ToString();
                cf.DateCreated = i.CreationTime.ToShortDateString();
                cf.Extension = i.Extension;
                cf.Status = ClientFiles.STATUS_SAVE_RUS;
                    
                clientFiles.Add(cf);
            }
            return clientFiles;
        }

       
        public static void SaveInPatientCardDirectory(string patientCardNumber, ClientFiles file)
        {
             var newPath = Path.Combine(GetPathToProgrammDirectory(), patientCardNumber, (file.FullName));
            File.Copy(file.Path, newPath, true);

        }

        public static List<string> Errors { get; set; } = new List<string>();
        public static bool HasErrors() => Errors.Count > 0;
        


        /*
        public static string GetFilesFromPatientCardDirectory()
        {

        }
*/
    }
}
