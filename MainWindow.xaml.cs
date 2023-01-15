using Dental.Models;
using Dental.Services;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Navigation;
using System;
using System.IO;
using System.Windows;
using IntelliLock.Licensing;
using Dental.Views.About;
using Dental.ViewModels;

namespace Dental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            try
            {              
                InitializeComponent();
                new Action(() =>
                {
                    IntelliLock.Licensing.EvaluationMonitor.LicenseCheckFinished += () =>
                    {
                        bool licenseAvailable = (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed);
                        bool hardwareIDMatches = (HardwareID.GetHardwareID(true, true, false, true, true, false) == EvaluationMonitor.CurrentLicense.HardwareID);
                        string licenseStatus = IntelliLock.Licensing.CurrentLicense.License.LicenseStatus.ToString();

                        if (licenseAvailable) // ���� �������� ��������
                        {
                            if (!hardwareIDMatches)
                            {
                                string confirmation_code = License_DeActivator.DeactivateLicense();
                                ThemedMessageBox.Show(title: "��������", text: "�������������������� ����� ���������! ��� ���� ����� ���������� ������������ ���������, �� ���������� ������.",
                                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);                              

                                IntelliLock.Licensing.EvaluationMonitor.LicenseExpired += () => 
                                { 
                                    new LicenseExpiredWindow() { DataContext = new LicExpiredViewModel() }.ShowDialog();
                                    Environment.Exit(0);
                                };
                            }
                        }

                        if (!licenseAvailable && licenseStatus == "2") // ���� ����� 
                        {
                            int expirationDays = EvaluationMonitor.CurrentLicense.ExpirationDays; // ������� ����� ���� �������� �� �����
                            int expirationDaysCurrent = EvaluationMonitor.CurrentLicense.ExpirationDays_Current; // ����� �� ����� ������ ���� ������
                            if (expirationDaysCurrent > expirationDays)
                            {
                                ThemedMessageBox.Show(title: "��������", text: "������� ������ �����! ��� ���� ����� ���������� ������������ ���������, �� ���������� ������.",
                                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

                                IntelliLock.Licensing.EvaluationMonitor.LicenseExpired += () =>
                                {
                                    new LicenseExpiredWindow() { DataContext = new LicExpiredViewModel() }.ShowDialog();
                                    Environment.Exit(0);
                                };
                            }
                        }
                    };
                }).BeginInvoke(null, null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message );
                Environment.Exit(0);
            }

        }

        private void ThemedWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string mes = "��������� ������ � �����������?";
          /*  if (Navigator.HasUnsavedChanges != null && Navigator.UserSelectedBtnCancel != null && Navigator.HasUnsavedChanges.Invoke()) mes = "������� ������������� ���������. " + mes; */

            var response = ThemedMessageBox.Show(title: "��������", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            if (response.ToString() == "No") 
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;

           Application.Current.Shutdown();         
        }

        private void stat_Click(object sender, EventArgs e) => stat.IsSelected = true;

        private void temp_Click(object sender, EventArgs e) => templ.IsSelected = true;
        
    }
}
