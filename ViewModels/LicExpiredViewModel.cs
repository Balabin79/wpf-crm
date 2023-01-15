﻿using System;
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
    public class LicExpiredViewModel : DevExpress.Mvvm.ViewModelBase
    {
        public LicExpiredViewModel()
        {
            try
            {
                string hardwareId = Status.HardwareID; // текущее Hardware ID
                Hardware_ID = hardwareId.Length > 100 ? "Недоступно" : hardwareId;
            }
            catch(Exception e)
            {

            }
        }


        [Command]
        public void Load(object p)
        {
            //InvalidateLicense();
            string filename = p?.ToString();
            if (string.IsNullOrEmpty(filename)) return;
            LoadLicense(filename);

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
                Status.LoadLicense(license);
            }

        }

        public bool IsValidLicenseAvailable()
        {
            return Status.Licensed;
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