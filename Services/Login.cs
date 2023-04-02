using Dental.Models;
using Dental.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                catch (Exception e)
                {
                    Log.ErrorHandler(e);
                    IsRoleAccessEnabled = false;
                    IsPasswordRequired = false;
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
                                case "ShowSheduler": UserSession.ShowSheduler = HasAccess(role); break;
                                case "AppointmentEditable": UserSession.AppointmentEditable = HasAccess(role); break;
                                case "AppointmentDeletable": UserSession.AppointmentDeletable = HasAccess(role); break;
                                case "PrintSheduler": UserSession.PrintSheduler = HasAccess(role); break;
                                case "ShedulerStatusEditable": UserSession.ShedulerStatusEditable = HasAccess(role); break;
                                case "ShedulerLocationEditable": UserSession.ShedulerLocationEditable = HasAccess(role); break;
                                case "ShedulerWorkTimeEditable": UserSession.ShedulerWorkTimeEditable = HasAccess(role); break;

                                case "ShowClients": UserSession.ShowClients = HasAccess(role); break;
                                case "ClientsEditable": UserSession.ClientsEditable = HasAccess(role); break;
                                case "ClientsDelitable": UserSession.ClientsDelitable = HasAccess(role); break;
                                case "InvoiceEditable": UserSession.InvoiceEditable = HasAccess(role); break;
                                case "InvoiceDelitable": UserSession.InvoiceDelitable = HasAccess(role); break;
                                case "PrintInvoice": UserSession.PrintInvoice = HasAccess(role); break;
                                case "PlanEditable": UserSession.PlanEditable = HasAccess(role); break;
                                case "PlanDelitable": UserSession.PlanDelitable = HasAccess(role); break;
                                case "PrintPlan": UserSession.PrintPlan = HasAccess(role); break;
                                case "ClientsImport": UserSession.ClientsImport = HasAccess(role); break;
                                case "PrintClients": UserSession.PrintClients = HasAccess(role); break;
                                case "ClientsAddFieldsEditable": UserSession.ClientsAddFieldsEditable = HasAccess(role); break;
                                case "ClientsCategoryEditable": UserSession.ClientsCategoryEditable = HasAccess(role); break;
                                case "ClientsAdvertisingEditable": UserSession.ClientsAdvertisingEditable = HasAccess(role); break;

                                case "ShowEmployees": UserSession.ShowEmployees = HasAccess(role); break;
                                case "EmployeeEditable": UserSession.EmployeeEditable = HasAccess(role); break;
                                case "EmployeeDelitable": UserSession.EmployeeDelitable = HasAccess(role); break;
                                case "PrintEmployees": UserSession.PrintEmployees = HasAccess(role); break;
                                case "EmployeeImport": UserSession.EmployeeImport = HasAccess(role); break;

                                case "ShowPrices": UserSession.ShowPrices = HasAccess(role); break;
                                case "PriceEditable": UserSession.PriceEditable = HasAccess(role); break;
                                case "PriceDelitable": UserSession.PriceDelitable = HasAccess(role); break;
                                case "PrintPrices": UserSession.PrintPrices = HasAccess(role); break;

                                case "ShowDocuments": UserSession.ShowDocuments = HasAccess(role); break;
                                case "DocumentImport": UserSession.DocumentImport = HasAccess(role); break;
                                case "DocumentEditable": UserSession.DocumentEditable = HasAccess(role); break;
                                case "DocumentDelitable": UserSession.DocumentDelitable = HasAccess(role); break;
                                case "PrintDocument": UserSession.PrintDocument = HasAccess(role); break;

                                case "ShowStatistics": UserSession.ShowStatistics = HasAccess(role); break;
                                case "ShowSettings": UserSession.ShowSettings = HasAccess(role); break;

                                case "LoadPrices": UserSession.ImportData = HasAccess(role); break;
                                case "UnloadPrices": UserSession.ExportData = HasAccess(role); break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorHandler(e);
                }
            }
            // SetUserSession();
            CanLoginWinClosing = true;
            LoginWin?.Close();
        }

        private void SetUserSessionForAdmin()
        {
            UserSession.ShowSheduler = true;
            UserSession.AppointmentEditable = true;
            UserSession.AppointmentDeletable = true;
            UserSession.PrintSheduler = true;
            UserSession.ShedulerStatusEditable = true;
            UserSession.ShedulerLocationEditable = true;
            UserSession.ShedulerWorkTimeEditable = true;

            UserSession.ShowClients = true;
            UserSession.ClientsEditable = true;
            UserSession.ClientsDelitable = true;
            UserSession.InvoiceEditable = true;
            UserSession.InvoiceDelitable = true;
            UserSession.PrintInvoice = true;
            UserSession.PlanEditable = true;
            UserSession.PlanDelitable = true;
            UserSession.PrintPlan = true;
            UserSession.ClientsImport = true;
            UserSession.PrintClients = true;
            UserSession.ClientsAddFieldsEditable = true;
            UserSession.ClientsCategoryEditable = true;
            UserSession.ClientsAdvertisingEditable = true;

            UserSession.ShowEmployees = true;
            UserSession.EmployeeEditable = true;
            UserSession.EmployeeDelitable = true;
            UserSession.PrintEmployees = true;
            UserSession.EmployeeImport = true;

            UserSession.ShowPrices = true;
            UserSession.PriceEditable = true;
            UserSession.PriceDelitable = true;
            UserSession.PrintPrices = true;

            UserSession.ShowDocuments = true;
            UserSession.DocumentImport = true;
            UserSession.DocumentEditable = true;
            UserSession.DocumentDelitable = true;
            UserSession.PrintDocument = true;

            UserSession.ShowStatistics = true;
            UserSession.ShowSettings = true;      
            UserSession.ImportData = true;      
            UserSession.ExportData = true;      
        }

        public bool HasAccess(RoleManagment role) => (Employee?.IsDoctor == 1 && role.DoctorAccess == 1) || (Employee?.IsReception == 1 && role.ReceptionAccess == 1);


        /*  Password */

        private bool PasswordValidate()
        {
            try
            {
                return Employee?.Password == BitConverter.ToString(MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(Password))).Replace("-", string.Empty);
            }
            catch(Exception e) { Log.ErrorHandler(e); return false; }
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
