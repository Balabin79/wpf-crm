using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using IntelliLock;
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
                LicenseAvailable = IsValidLicenseAvailable();
                HardwareIDMatches = CompareHardwareID();
                LockEnabled = EvaluationMonitor.CurrentLicense.ExpirationDays_Enabled; // включена ли блокировка            

                TrialStatus = "Незарегистрированная копия";

                if (LicenseAvailable) // если доступна лицензия
                {
                    if (HardwareIDMatches && LicenseStatus == "1")
                    {
                        TrialStatus = "Зарегистрированная копия";
                    }
                    if (!HardwareIDMatches)
                    {
                        TrialStatus = "Незарегистрированная копия";
                    }                   
                } 
                if (!LicenseAvailable && LicenseStatus == "2") // если триал 
                {
                    ExpirationDays = EvaluationMonitor.CurrentLicense.ExpirationDays; // сколько всего дней отведено на триал
                    ExpirationDaysCurrent = EvaluationMonitor.CurrentLicense.ExpirationDays_Current; // какой по счету сейчас день триала
                    TrialStatus = "Пробный период, осталось дней: " + ExpirationDays;
                }

                Msg = new IntelliLock.DialogBoxAttribute();
                /*****/
                ReadAdditonalLicenseInformation(); // информация о компании, фио, дате выдачи
                string hardwareId = HardwareID.GetHardwareID(true, true, false, true, true, false); // текущее Hardware ID
                Hardware_ID = hardwareId.Length > 100 ? "Недоступно" : hardwareId;

            }
            catch (Exception e)
            {

            }
        }

        public DialogBoxAttribute Msg { get; set; }
        public bool LockEnabled { get; set; }
        public bool HardwareIDMatches { get; set; }
        public bool LicenseAvailable { get; set; }

        public int ExpirationDays { get; set; }
        public int ExpirationDaysCurrent { get; set; }

        public string LicHardwareId { get; set; }

        // сравнение текущего Hardware ID и Hardware ID для которого выдана лицензия
        public bool CompareHardwareID()
        {
            if (HardwareID.GetHardwareID(true, true, false, true, true, false) == EvaluationMonitor.CurrentLicense.HardwareID)
                return true;
            else
                return false;
        }

        [Command]
        public void Load(object p)
        {
            //InvalidateLicense();
            string filename = p?.ToString();
            if (string.IsNullOrEmpty(filename)) return;
            LoadLicense(filename);
        }

        public void LoadLicense(string filename)
        {
            //EvaluationMonitor.LoadLicense(filename);
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                // Create a new instance of memorystream
                var memoryStream = new MemoryStream();

                // Use the .CopyTo() method and write current filestream to memory stream
                stream.CopyTo(memoryStream);

                byte[] license = memoryStream.ToArray();
                LoadLicense(license);
            }

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

        // получение Hardware ID для которого выдана лицензия
        public void CheckHardwareLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.HardwareLock_Enabled;

            if (lock_enabled)
            {
                /* Get Hardware ID stored in the license file */
                LicHardwareId = EvaluationMonitor.CurrentLicense.HardwareID;
            }
        }

        public void InvalidateLicense()
        {
            string confirmation_code = License_DeActivator.DeactivateLicense();
        }

        public void LoadLicense(byte[] license)
        {
            EvaluationMonitor.LoadLicense(license);
        }

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

        public string Hardware_ID
        {
            get { return GetProperty(() => Hardware_ID); }
            set { SetProperty(() => Hardware_ID, value); }
        }

        public string TrialStatus
        {
            get { return GetProperty(() => TrialStatus); }
            set { SetProperty(() => TrialStatus, value); }
        }
    }
}
