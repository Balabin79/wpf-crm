using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using IntelliLock.Licensing;

namespace Dental.ViewModels
{
    public class LicExpiredViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public LicExpiredViewModel()
        {
            try
            {
                string hardwareId = HardwareID.GetHardwareID(true, true, false, true, true, false); // текущее Hardware ID
                Hardware_ID = hardwareId.Length > 100 ? "Недоступно" : hardwareId;
            }
            catch(Exception e)
            {

            }
        }

        [Command]
        public void Load(object p)
        {
            string filename = p?.ToString();
            if (string.IsNullOrEmpty(filename)) return;
            EvaluationMonitor.LoadLicense(filename);

            bool hardwareIDMatches = (HardwareID.GetHardwareID(true, true, false, true, true, false) == EvaluationMonitor.CurrentLicense.HardwareID);
            string status = IntelliLock.Licensing.CurrentLicense.License.LicenseStatus.ToString();
           
            if (!hardwareIDMatches)
            {
                string confirmation_code = License_DeActivator.DeactivateLicense();
                ThemedMessageBox.Show(title: "Внимание", text: "Некорректный файл лицензии!",
                  messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            }

            if (hardwareIDMatches && status == "1")
            {
                ThemedMessageBox.Show(title: "Внимание", text: "Спасибо за регистрацию!",
                  messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            }
        }


        public bool IsValidLicenseAvailable()
        {
            return (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed);
        }

        public string GetLicenseStatus() => IntelliLock.Licensing.CurrentLicense.License.LicenseStatus.ToString();



        public void InvalidateLicense()
        {
            string confirmation_code = License_DeActivator.DeactivateLicense();
        }

        public string LicenseStatus
        {
            get { return GetProperty(() => LicenseStatus); }
            set { SetProperty(() => LicenseStatus, value); }
        }

        public string Hardware_ID
        {
            get { return GetProperty(() => Hardware_ID); }
            set { SetProperty(() => Hardware_ID, value); }
        }

    }
}
