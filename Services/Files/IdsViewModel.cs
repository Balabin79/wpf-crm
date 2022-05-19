using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Dental.Services.Files
{
    public class IdsViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public IdsViewModel(string path)
        {
            PathTo = path;
            Files = GetFiles().ToObservableCollection();
        }

        virtual public IEnumerable<FileInfo> GetFiles()
        {
            try
            {
                return new DirectoryInfo(PathTo).GetFiles();
            }
            catch
            {
                return new FileInfo[] { };
            }
        }

        virtual protected string PathTo { get; }

        public ObservableCollection<FileInfo> Files
        {
            get { return GetProperty(() => Files); }
            set { SetProperty(() => Files, value); }
        }
    }
}
