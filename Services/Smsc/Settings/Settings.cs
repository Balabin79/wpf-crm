using Dental.Models;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services.Smsc.Settings
{
    public class Settings : BaseSettings
    {

        public Settings(PatientInfo[] clients)
        {
            try
            {

            } catch(Exception e)
            {
                ThemedMessageBox.Show(title: "Ошибка", text: e.Message, messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }
    }
}
