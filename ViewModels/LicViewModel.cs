using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using IntelliLock.Licensing;

namespace Dental.ViewModels
{
    public class LicViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public LicViewModel()
        {
            try
            {
                LicenseStatus = GetLicenseStatus();
                ReadAdditonalLicenseInformation();
                string hardwareId = IntelliLock.Licensing.HardwareID.GetHardwareID(true, true, false, true, true, false);
                HardwareID = hardwareId.Length > 100 ? "Недоступно" : hardwareId;
                TrialDate = EvaluationMonitor.CurrentLicense.ExpirationDate.ToShortDateString();
            }
            catch(Exception e)
            {

            }
        }

        [Command]
        public void LoadLicense(object p)
        {
            string filename = p?.ToString();
            if (string.IsNullOrEmpty(filename)) return;
            EvaluationMonitor.LoadLicense(filename);
        }

        public void ReadAdditonalLicenseInformation()
        {
            try
            {
                if (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed)
                {
                    /* Read additional license information */
                    //for (int i = 0; i < EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                   // {
                    Company = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(0).ToString();
                    Date = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(1).ToString();
                    FullName = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(2).ToString();
                    //}
                }
            }
            catch
            {
                Company = "Недоступно";
                Date = "Недоступно";
                FullName = "Недоступно";
            }
            /* Check first if a valid license file is found */

        }

        public bool IsValidLicenseAvailable()
        {
            return (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed);
        }

        public string GetLicenseStatus() => IntelliLock.Licensing.CurrentLicense.License.LicenseStatus.ToString();   

        public string LicenseStatus
        {
            get { return GetProperty(() => LicenseStatus); }
            set { SetProperty(() => LicenseStatus, value); }
        }

        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }

        public string FullName
        {
            get { return GetProperty(() => FullName); }
            set { SetProperty(() => FullName, value); }
        }

        public string Company
        {
            get { return GetProperty(() => Company); }
            set { SetProperty(() => Company, value); }
        }

        public string HardwareID
        {
            get { return GetProperty(() => HardwareID); }
            set { SetProperty(() => HardwareID, value); }
        }

        public string TrialDate
        {
            get { return GetProperty(() => TrialDate); }
            set { SetProperty(() => TrialDate, value); }
        }
    }
}
