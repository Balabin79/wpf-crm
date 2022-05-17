using System.Collections.Generic;
using System.IO;

namespace Dental.Services.Files
{
    public abstract class AbstractFilesManagement
    {
        public AbstractFilesManagement(string path) => PathTo = path;

        virtual public IEnumerable<FileInfo> GetFiles()
        {
            try
            {
                return new DirectoryInfo(PathTo).GetFiles();
            }
            catch
            {
                return new FileInfo[]{};
            }          
        }

        virtual public DirectoryInfo CreateDirectory()
        {
            try
            {
                return Directory.Exists(PathTo) ? Directory.CreateDirectory(PathTo) : new DirectoryInfo(PathTo);
            }
            catch
            {
                return new DirectoryInfo(PathTo);
            }
        }

        virtual public void DeleteDirectory() => Directory.Delete(PathTo, true);

        virtual public void RemoveFile(FileInfo file) => file.Delete();

        virtual public void SaveFile(FileInfo file) => File.Copy(file.FullName, Path.Combine(PathTo, file.FullName), true);

        virtual protected string PathTo { get; }
    }
}
