using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliLock.Licensing;

namespace Dental.Services
{
    public class LManager
    {
        public void Run()
        {
            bool x = IsValidLicenseAvailable();
        }



        /*** Check if a valid license file is available. ***/
        public bool IsValidLicenseAvailable()
        {
            return (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed);
        }



        //Read license information from a license file
        /*** Read additonal license information from a license file ***/
        public void ReadAdditonalLicenseInformation()
        {
            /* Check first if a valid license file is found */
            if (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed)
            {
                /* Read additional license information */
                for (int i = 0; i < EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                {
                    string key = EvaluationMonitor.CurrentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(i).ToString();
                }
            }
        }


        //Check the license status of the Expiration Days Lock
        /*** Check the license status of the Expiration Days Lock ***/
        public void CheckExpirationDaysLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.ExpirationDays_Enabled;
            int days = EvaluationMonitor.CurrentLicense.ExpirationDays;
            int days_current = EvaluationMonitor.CurrentLicense.ExpirationDays_Current;
        }


        //Check the license status of the Expiration Date Lock
        /*** Check the license status of the Expiration Date Lock ***/
        public void CheckExpirationDateLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.ExpirationDate_Enabled;
            System.DateTime expiration_date = EvaluationMonitor.CurrentLicense.ExpirationDate;
        }


        //Check the license status of the Executions Lock
        /*** Check the license status of the Executions Lock ***/
        public void CheckExecutionsLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.Executions_Enabled;
            int max_executions = EvaluationMonitor.CurrentLicense.Executions;
            int current_executions = EvaluationMonitor.CurrentLicense.Executions_Current;
        }


        //Check the license status of the Instances Lock
        /*** Check the license status of the Instances Lock ***/
        public void CheckNumberOfInstancesLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.Instances_Enabled;
            int max_instances = EvaluationMonitor.CurrentLicense.Instances;
        }


        //Check the license status of Hardware Lock
        /*** Check the license status of Hardware Lock ***/
        public void CheckHardwareLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.HardwareLock_Enabled;

            if (lock_enabled)
            {
                /* Get Hardware ID stored in the license file */
                string lic_hardware_id = EvaluationMonitor.CurrentLicense.HardwareID;
            }
        }


        //Get Hardware ID of the current machine
        /*** Get Hardware ID of the current machine ***/
        public string GetHardwareID()
        {
            return HardwareID.GetHardwareID(true, true, false, true, true, false);
        }


        //Compare current Hardware ID with Hardware ID stored in License File
        /*** Compare current Hardware ID with Hardware ID stored in License File ***/
        public bool CompareHardwareID()
        {
            if (HardwareID.GetHardwareID(true, true, false, true, true, false) == EvaluationMonitor.CurrentLicense.HardwareID)
                return true;
            else
                return false;
        }


        //Invalidate the license
        /*** Invalidate the license. Please note, your protected software does not accept a license file anymore! ***/
        public void InvalidateLicense()
        {
            string confirmation_code = License_DeActivator.DeactivateLicense();
        }


        //Reactivate the license
        /*** Reactivate an invalidated license. ***/
        public bool ReactivateLicense(string reactivation_code)
        {
            return License_DeActivator.ReactivateLicense(reactivation_code);
        }


        //Manually load a license using a filename
        /*** Load the license. ***/
        public void LoadLicense(string filename)
        {
            EvaluationMonitor.LoadLicense(filename);
        }


        //Manually load a license using byte[]
        /*** Load the license. ***/
        public void LoadLicense(byte[] license)
        {
            EvaluationMonitor.LoadLicense(license);
        }


        //Get loaded license(if available) as byte[]
        /*** Get the license. ***/
        public byte[] GetLicense()
        {
            return EvaluationMonitor.GetCurrentLicenseAsByteArray();
        }
    }
}
