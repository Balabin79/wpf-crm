using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Core;
using System.Windows;
using Npgsql;
using B6CRM.Views.WindowForms;
using B6CRM.ViewModels;

namespace B6CRM.Services
{
    public static class Log
    {
        public static void ErrorHandler(
            Exception e,
            string publicError = "Произошла ошибка!",
            bool showMsg = false,
            bool isRedirect = false,
            string page = ""
            )
        {   
            if (showMsg) ThemedMessageBox.Show(
                title: "Ошибка", 
                text: publicError + " Возможно, что данные были кем-то удалены или изменены во время сессии, перезагрузите раздел.", 
                messageBoxButtons: MessageBoxButton.OK, 
                icon: MessageBoxImage.Error
                );

            string path = Path.Combine(Config.defaultPath, "log.txt");
            string msg = DateTime.Now + "\n" + e.Message + "\n" + e.Source + "\n\n";
            File.AppendAllText(path, msg);
        }
    }
}
