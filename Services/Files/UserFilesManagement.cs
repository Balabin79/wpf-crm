using Dental.Infrastructures.Logs;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Dental.Services.Files
{
    class UserFilesManagement : AbstractFilesManagement
    {
        public UserFilesManagement(string Guid) : base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FILES, Guid)){}

        private const string FILES = "B6\\Files";
    }
}
