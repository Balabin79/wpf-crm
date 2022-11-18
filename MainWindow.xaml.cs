using Dental.Models;
using Dental.Services;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using System;
using System.Windows;

namespace Dental
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            Login();
            InitializeComponent();
        }

        private void Restart()
        {
            string mes = "��������� ����� ������?";
            var response = ThemedMessageBox.Show(title: "��������", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            if (response.ToString() == "No") return;
            Login();
            var userSession = (UserSession)Application.Current.Resources["UserSession"];

                var nav = (Navigator)Application.Current.Resources["Router"];

                sheduleBtn.IsVisible = userSession.SheduleRead;
                clientsBtn.IsVisible = userSession.ClientsRead;
                employeesBtn.IsVisible = userSession.EmployeesRead;
                servicesBtn.IsVisible = userSession.PricesRead;
                templatesBtnItem.IsVisible = userSession.TemplatesRead;
                settingsBtnItem.IsVisible = userSession.SettingsRead;
                orgBtnItem.IsVisible = userSession.OrgRead;

                if (userSession.ClientsRead) { nav.LeftMenuClick("Dental.Views.PatientCard.PatientsList"); return;  }
                if (userSession.SheduleRead) { nav.LeftMenuClick("Dental.Views.Sheduler"); return; }
                if (userSession.EmployeesRead) { nav.LeftMenuClick("Dental.Views.Sheduler"); return; }
                if (userSession.PricesRead) { nav.LeftMenuClick("Dental.Views.ServicePrice.ServicePage"); return; }
                if (userSession.TemplatesRead) { nav.LeftMenuClick("Dental.Views.Templates.MainPage"); return; }
                if (userSession.SettingsRead) { nav.LeftMenuClick("Dental.Views.Settings.SettingsPage"); return; }
                if (userSession.OrgRead) { nav.LeftMenuClick("Dental.Views.Dental.Views.Organization"); return; }
        }

        private void Login()
        {
            try
            {
                var login = new Login();
                login.ShowLogin();
                Application.Current.Resources["UserSession"] = login.UserSession;
            }
            catch (Exception e)
            {

            }
        }

        private void ThemedWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string mes = "��������� ������ � �����������?";
            if (Navigator.HasUnsavedChanges != null && Navigator.UserSelectedBtnCancel != null && Navigator.HasUnsavedChanges.Invoke()) mes = "������� ������������� ���������. " + mes; 

            var response = ThemedMessageBox.Show(title: "��������", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            if (response.ToString() == "No") 
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;

           Application.Current.Shutdown();  
          
        }

        private void BarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            Restart();
        }
    }
}
