using Dental.Models;
using Dental.Services;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using System;
using System.IO;
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
            try
            {
                Login();
                InitializeComponent();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message + " WWW " + ex.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " WWW " + e.InnerException.Message);
            }

        }

        private void Restart()
        {
            string mes = "Завершить сеанс работы?";
            var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
            if (response.ToString() == "No") return;
            Login();
            var userSession = (UserSession)Application.Current.Resources["UserSession"];

                //var nav = (Navigator)Application.Current.Resources["Router"];

                statBtnItem.IsVisible = userSession.StatisticRead;
                sheduleBtn.IsVisible = userSession.SheduleRead;
                clientsBtn.IsVisible = userSession.ClientsRead;
                employeesBtn.IsVisible = userSession.EmployeesRead;
                servicesBtn.IsVisible = userSession.PricesRead;
                templatesBtnItem.IsVisible = userSession.TemplatesRead;
                settingsBtnItem.IsVisible = userSession.SettingsRead;
                orgBtnItem.IsVisible = userSession.OrgRead;

                /*if (userSession.ClientsRead) { nav.LeftMenuClick("Dental.Views.PatientCard.PatientsList"); return;  }
                if (userSession.SheduleRead) { nav.LeftMenuClick("Dental.Views.Sheduler"); return; }
                if (userSession.EmployeesRead) { nav.LeftMenuClick("Dental.Views.Sheduler"); return; }
                if (userSession.PricesRead) { nav.LeftMenuClick("Dental.Views.ServicePrice.ServicePage"); return; }
                if (userSession.TemplatesRead) { nav.LeftMenuClick("Dental.Views.Templates.MainPage"); return; }
                if (userSession.SettingsRead) { nav.LeftMenuClick("Dental.Views.Settings.SettingsPage"); return; }
                if (userSession.StatisticRead) { nav.LeftMenuClick("Dental.Views.Statistic.StatisticPage"); return; }
                if (userSession.OrgRead) { nav.LeftMenuClick("Dental.Views.Dental.Views.Organization"); return; }*/
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
          /*  if (Navigator.HasUnsavedChanges != null && Navigator.UserSelectedBtnCancel != null && Navigator.HasUnsavedChanges.Invoke()) mes = "Имеются несохраненные изменения. " + mes; */

            var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);

            if (response.ToString() == "No") 
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;

           Application.Current.Shutdown();         
        }

        private void BarButtonItem_ItemClick(object sender, ItemClickEventArgs e) => Restart();   
    }
}
