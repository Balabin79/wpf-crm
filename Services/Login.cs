using Dental.Models;
using Dental.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;

namespace Dental.Services
{
    public class Login : ViewModelBase
    {
        public Login()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    Setting = db.Settings.FirstOrDefault();
                    IsRoleAccessEnabled = Setting?.RolesEnabled == 1;
                    IsPasswordRequired = Setting?.IsPasswordRequired == 1;
                    Employees = db.Employes.OrderBy(f => f.LastName).ToArray();                    
                }
                catch(Exception e)
                {

                    IsRoleAccessEnabled = false;
                    IsPasswordRequired = false;
                    ThemedMessageBox.Show(title: "Ошибка", text: e.InnerException.Message + " WWW " + e.StackTrace + " WWW " + e.Source + " WWW " + e.Message,
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                }            
            }
        }

        public void ShowLogin()
        {
            if (IsRoleAccessEnabled)
            {
                LoginWin = new LoginWin() { DataContext = this };
                LoginWin.ShowDialog();
            }
            else 
            {
                UserSession = new UserSession();
                SetUserSessionForAdmin();
                //SetUserSession();
            }
        }

       /* public void SetUserSession()
        {
           try
           {
               Application.Current.Resources["UserSession"] = UserSession;
           }
            catch(Exception e)
           {

           }                      
        }*/

        [Command]
        public void CloseForm(object p)
        {
            if (CanLoginWinClosing) return;
            if (p is CancelEventArgs args)
            {
                if (CanLoginWinClosing) 
                {
                    CanLoginWinClosing = false;
                    return;
                }

                var result = ThemedMessageBox.Show(title: "Завершение сеанса", text: "Завершить сеанс!",
                    messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (result.ToString() == "No")
                {
                    args.Cancel = true;
                    return;
                } 
                else App.Current.Shutdown();
            }
        }

        [Command]
        public void Auth(object p)
        {
            UserSession = new UserSession();
            if (Employee == null) return;
            UserSession.Employee = Employee;

            if (IsPasswordRequired == true)
            {
                if (string.IsNullOrEmpty(Employee?.Password))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Для данного пользователя не задан пароль в карте сотрудника!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Password))
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Поле пароля пустое!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Exclamation);
                    return;
                }
                if (!PasswordValidate())
                {
                    ThemedMessageBox.Show(title: "Ошибка", text: "Не совпадает пароль, повторите попытку!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Exclamation);
                    return;
                }
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    var roles = db.RolesManagment.ToArray();

                    if (Employee.IsAdmin == 1) SetUserSessionForAdmin();
                    else
                    {
                        foreach (var role in roles)
                        {
                            switch (role?.PageName)
                            {
                                case "ClientsRead": UserSession.ClientsRead = HasAccess(role); break;
                                case "ClientEditable": UserSession.ClientEditable = HasAccess(role); break;
                                case "ClientDeletable": UserSession.ClientDeletable = HasAccess(role); break;
                                case "ClientTemplatesEditable": UserSession.ClientTemplatesEditable = HasAccess(role); break;
                                case "ClientAddFieldsEditable": UserSession.ClientEditable = HasAccess(role); break;

                                case "EmployeesRead": UserSession.EmployeesRead = HasAccess(role); break;
                                case "EmployeeEditable": UserSession.EmployeeEditable = HasAccess(role); break;
                                case "EmployeeDeletable": UserSession.EmployeeDeletable = HasAccess(role); break;

                                case "SheduleRead": UserSession.SheduleRead = HasAccess(role); break;
                                case "SheduleStatusEditable": UserSession.SheduleStatusEditable = HasAccess(role); break;
                                case "SheduleStatusDeletable": UserSession.SheduleStatusDeletable = HasAccess(role); break;
                                case "SheduleLocationEditable": UserSession.SheduleLocationEditable = HasAccess(role); break;
                                case "SheduleLocationDeletable": UserSession.SheduleLocationDeletable = HasAccess(role); break;

                                case "PricesRead": UserSession.PricesRead = HasAccess(role); break;
                                case "PriceEditable": UserSession.PriceEditable = HasAccess(role); break;
                                case "PriceDeletable": UserSession.PriceDeletable = HasAccess(role); break;

                                case "TemplatesRead": UserSession.TemplatesRead = HasAccess(role); break;
                                case "TemplateEditable": UserSession.TemplateEditable = HasAccess(role); break;
                                case "TemplateDeletable": UserSession.TemplateDeletable = HasAccess(role); break;

                                case "SettingsRead": UserSession.SettingsRead = HasAccess(role); break;

                                case "OrgRead": UserSession.OrgRead = HasAccess(role); break;
                                case "OrgEditable": UserSession.OrgEditable = HasAccess(role); break;
                                case "OrgDeletable": UserSession.OrgDeletable = HasAccess(role); break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
           // SetUserSession();
            CanLoginWinClosing = true;
            LoginWin?.Close();
        }

        private void SetUserSessionForAdmin()
        {
            UserSession.ClientsRead = true;
            UserSession.ClientEditable = true;
            UserSession.ClientDeletable = true;
            UserSession.ClientTemplatesEditable = true;

            UserSession.EmployeesRead = true;
            UserSession.EmployeeEditable = true;
            UserSession.EmployeeDeletable = true;

            UserSession.SheduleRead = true;
            UserSession.SheduleStatusEditable = true;
            UserSession.SheduleStatusDeletable = true;
            UserSession.SheduleLocationEditable = true;
            UserSession.SheduleLocationDeletable = true;

            UserSession.PricesRead = true;
            UserSession.PriceEditable = true;
            UserSession.PriceDeletable = true;

            UserSession.TemplatesRead = true;
            UserSession.TemplateEditable = true;
            UserSession.TemplateDeletable = true;

            UserSession.SettingsRead = true;

            UserSession.OrgRead = true;
            UserSession.OrgEditable = true;
            UserSession.OrgDeletable = true;
        }

        public bool HasAccess(RoleManagment role) => (Employee?.IsDoctor == 1 && role.DoctorAccess == 1) || (Employee?.IsReception == 1 && role.ReceptionAccess == 1);   
        

        /*  Password */

        private bool PasswordValidate()
        {
            try
            {
                return Employee?.Password == BitConverter.ToString(MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(Password))).Replace("-", string.Empty);
            }
            catch
            {
                return false;
            }
        }

        /*******************************************/


        public Setting Setting { get; set; }
        public UserSession UserSession { get; set; } 

        public Employee Employee
        {
            get { return GetProperty(() => Employee); }
            set { SetProperty(() => Employee, value); }
        }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value); }
        }


        public ICollection<Employee> Employees { get; set; }
        public bool IsPasswordRequired { get; set; }
        private bool IsRoleAccessEnabled { get; set; }
        private LoginWin LoginWin { get; set; }
        private bool CanLoginWinClosing { get; set; } = false;
        
    }
    public enum Roles { Guest = -1, Doctor = 1, Reception = 2, Admin = 3 }
}
