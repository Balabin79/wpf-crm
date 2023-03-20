using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Dental.Services
{
    public static class Log 
    {
        public static void ErrorHandler(
            Exception e, 
            string publicError = "Произошла ошибка!", 
            bool showMsg = false, 
            bool isRedirectOnSettings = false
            )
        {
            try
            {
                if (showMsg) ThemedMessageBox.Show(title: "Ошибка", text: publicError, messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);

                string path = Path.Combine(Config.defaultPath, "log.txt");
                string msg = DateTime.Now + "\n" + e.Message + "\n" + e.Source + "\n\n";
                File.AppendAllText(path, msg);           
            }
            catch { }
        }
    }
}
