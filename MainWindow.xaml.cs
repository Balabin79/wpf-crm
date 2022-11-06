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
            string mes = "Завершить сеанс работы?";
            var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            if (response.ToString() == "No") return;
            Login();
            var userSession = (UserSession)Application.Current.Resources["UserSession"];

                var nav = (Navigator)Application.Current.Resources["Router"];

                sheduleBtn.IsVisible = userSession.SheduleRead;
                clientsBtn.IsVisible = userSession.ClientsListRead;
                employeesBtn.IsVisible = userSession.EmployeesListRead;
                servicesBtn.IsVisible = userSession.ServicesRead;
                nomenclaturesBtn.IsVisible = userSession.NomenclaturesRead;
                templatesBtnItem.IsVisible = userSession.TemplatesRead;
                addFieldsBtnItem.IsVisible = userSession.AddFieldsRead;
                settingsBtnItem.IsVisible = userSession.SettingsRead;

                if (userSession.ClientsListRead) { nav.LeftMenuClick("Dental.Views.PatientCard.PatientsList"); return;  }
                if (userSession.SheduleRead) { nav.LeftMenuClick("Dental.Views.Sheduler"); return; }
                if (userSession.EmployeesListRead) { nav.LeftMenuClick("Dental.Views.Sheduler"); return; }
                if (userSession.ServicesRead) { nav.LeftMenuClick("Dental.Views.ServicePrice.ServicePage"); return; }
                if (userSession.NomenclaturesRead) { nav.LeftMenuClick("Dental.Views.NomenclatureDir.Nomenclature"); return; }
                if (userSession.TemplatesRead) { nav.LeftMenuClick("Dental.Views.Templates.MainPage"); return; }
                if (userSession.AddFieldsRead) { nav.LeftMenuClick("Dental.Views.AdditionalFields.AdditionalFieldsPage"); return; }
                if (userSession.SettingsRead) { nav.LeftMenuClick("Dental.Views.Settings.SettingsPage"); return; }
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
            string mes = "Завершить работу с приложением?";
            if (Navigator.HasUnsavedChanges != null && Navigator.UserSelectedBtnCancel != null && Navigator.HasUnsavedChanges.Invoke()) mes = "Имеются несохраненные изменения. " + mes; 

            var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

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
