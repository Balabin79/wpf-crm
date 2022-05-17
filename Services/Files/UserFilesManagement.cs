using System;
using System.IO;

namespace Dental.Services.Files
{
    class UserFilesManagement : AbstractFilesManagement
    {
        public UserFilesManagement(string Guid) : base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FILES, Guid)){}
        private const string FILES = "B6\\Files ";
    }
}
