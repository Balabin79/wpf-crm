using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using License;

namespace Dental.ViewModels
{
    public class LicViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public LicViewModel()
        {
            try
            {
                //LicenseStatus = GetLicenseStatus();
                LicenseAvailable = IsValidLicenseAvailable();
                HardwareIDMatches = CompareHardwareID();
                LockEnabled = Status.Evaluation_Lock_Enabled; // включена ли блокировка            

                TrialStatus = "Незарегистрированная копия";

                if (LicenseAvailable) // если доступна лицензия
                {
                    if (HardwareIDMatches/* && LicenseStatus == "1"*/)
                    {
                        TrialStatus = "Зарегистрированная копия";
                    }
                    if (!HardwareIDMatches)
                    {
                        TrialStatus = "Незарегистрированная копия";
                    }                   
                } 
                if (!LicenseAvailable /*&& LicenseStatus == "2"*/) // если триал 
                {
                    ExpirationDays = Status.Evaluation_Time; // сколько всего дней отведено на триал
                    ExpirationDaysCurrent = Status.Evaluation_Time_Current; // какой по счету сейчас день триала
                    int days = ExpirationDays - ExpirationDaysCurrent > 0 ? ExpirationDays - ExpirationDaysCurrent : 0;
                    TrialStatus = "Пробный период, осталось дней: " + days;
                }

                /*****/
                ReadAdditonalLicenseInformation(); // информация о компании, фио, дате выдачи
                string hardwareId = Status.HardwareID; // текущее Hardware ID
                Hardware_ID = hardwareId.Length > 100 ? "Недоступно" : hardwareId;

            }
            catch
            {
                
            }
        }

        public bool LockEnabled { get; set; }
        public bool HardwareIDMatches { get; set; }
        public bool LicenseAvailable { get; set; }

        public int ExpirationDays { get; set; }
        public int ExpirationDaysCurrent { get; set; }

        public string LicHardwareId { get; set; }

        // сравнение текущего Hardware ID и Hardware ID для которого выдана лицензия
        public bool CompareHardwareID()
        {
            if (Status.HardwareID == Status.License_HardwareID)
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

                bool hardwareIDMatches = (Status.HardwareID == Status.License_HardwareID);

                if (!hardwareIDMatches)
                {
                    ThemedMessageBox.Show(title: "Внимание", text: "Некорректный файл лицензии!",
                      messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                }

                if (hardwareIDMatches)
                {
                    ThemedMessageBox.Show(title: "Внимание", text: "Спасибо за регистрацию!",
                      messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    var file = new FileInfo(filename).Name;
                    File.Copy(filename, Path.Combine(Environment.CurrentDirectory.ToString(), file), true);
                }              
            }
            catch
            {
            }
        }

        public void ReadAdditonalLicenseInformation()
        {
            try
            {
                if (Status.Licensed)
                {
                    /* Read additional license information */
                    //for (int i = 0; i < EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                   // {
                    //Company = Status.KeyValueList.GetByIndex(0).ToString();
                    //FullName = Status.KeyValueList.GetByIndex(2).ToString();
                    //}
                }
            }
            catch
            {

            }
            /* Check first if a valid license file is found */

        }

        public bool IsValidLicenseAvailable()
        {
            return Status.Licensed;
        }

        //public string GetLicenseStatus() => Status. IntelliLock.Licensing.CurrentLicense.License.LicenseStatus.ToString();

        // получение Hardware ID для которого выдана лицензия
        public void CheckHardwareLock()
        {
            bool lock_enabled = Status.Hardware_Lock_Enabled;
            if (lock_enabled)
            {
                // Get Hardware ID which is stored inside the license file
                LicHardwareId = Status.License_HardwareID;
            }
        }

        public void InvalidateLicense()
        {
            string confirmation_code = Status.InvalidateLicense();
        }

        public void LoadLicense(byte[] license)
        {
            Status.LoadLicense(license);
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

        public string TrialStatus
        {
            get { return GetProperty(() => TrialStatus); }
            set { SetProperty(() => TrialStatus, value); }
        }
    }
}
