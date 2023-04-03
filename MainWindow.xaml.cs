using B6CRM.Models;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Navigation;
using System;
using System.IO;
using System.Windows;
using B6CRM.Views.About;
using License;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Threading.Tasks;
using System.Timers;
using B6CRM.Services;
using B6CRM.ViewModels;

namespace B6CRM
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
                var login = new Login();
                login.ShowLogin();

                Application.Current.Resources["UserSession"] = login.UserSession;

                //инициализация рассылки сообщений 
                // создаем таймер на 5 мин.
                Timer timer = new Timer() { Interval = 300000, Enabled = true, AutoReset = true };
                timer.Elapsed += OnTimedEvent;

                InitializeComponent();
                SetPageVisibility();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TelegramNotificationsSenderService.Send();
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

        private void stat_Click(object sender, EventArgs e) => stat.IsSelected = true;

        private void Restart()
        {
            try
            {
                string mes = "Завершить сеанс работы?";
                var response = ThemedMessageBox.Show(title: "Внимание", text: mes, messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Warning);
                if (response.ToString() == "No") return;

                Login();
                SetPageVisibility();

                if (DataContext is MainViewModel vm)
                    vm?.NavigationService?.Navigate("B6CRM.Views.PatientCard.PatientsList", null, this);
            }
            catch(Exception e) 
            {
                Log.ErrorHandler(e);
            }
        }

        private void SetPageVisibility()
        {
            var userSession = (UserSession)Application.Current.Resources["UserSession"];

            shedulerBtn.Visibility = userSession.ShowSheduler ? Visibility.Visible : Visibility.Collapsed;
            clientsBtn.Visibility = userSession.ShowClients ? Visibility.Visible : Visibility.Collapsed;
            employeesBtn.Visibility = userSession.ShowEmployees ? Visibility.Visible : Visibility.Collapsed;
            pricesBtn.Visibility = userSession.ShowPrices ? Visibility.Visible : Visibility.Collapsed;
            documentsBtn.Visibility = userSession.ShowDocuments ? Visibility.Visible : Visibility.Collapsed;
            settingsBtn.Visibility = userSession.ShowSettings ? Visibility.Visible : Visibility.Collapsed;
            stat.Visibility = userSession.ShowStatistics ? Visibility.Visible : Visibility.Collapsed;
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
                Log.ErrorHandler(e);
            }
        }

        private void AuthBtn_Click(object sender, EventArgs e) => Restart();
    }
}
