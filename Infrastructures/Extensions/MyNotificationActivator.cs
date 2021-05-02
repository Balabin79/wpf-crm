using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Infrastructures.Extensions
{
    [Guid("E343F8F2-CA68-4BF4-BB54-EEA4B3AC4A31"), ComVisible(true)]
    public class MyNotificationActivator : ToastNotificationActivator
    {
        public override void OnActivate(string arguments, Dictionary<string, string> data)
        {
            MessageBox.Show("Activate it!");
        }
    }
}
