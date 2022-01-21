using Dental.Services;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Windows;

namespace Dental
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ApplicationID
        {
            get { return "FunWithNotifications_19_1"; }
        }
    }

    [Guid("E343F8F2-CA68-4BF4-BB54-EEA4B3AC4A31"), ComVisible(true)]
    public class MyNotificationActivator : ToastNotificationActivator
    {
        public override void OnActivate(string arguments, Dictionary<string, string> data)
        {
            MessageBox.Show("Activate it!");
        }
    }
}
