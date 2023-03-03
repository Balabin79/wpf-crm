using Dental.Models;
using Dental.Views.WindowForms;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DevExpress.Mvvm.Native;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

namespace Dental.Services
{
    public class RtfParse : DevExpress.Mvvm.ViewModelBase
    {
        private readonly ApplicationContext db;
        public RtfParse(string txt)
        {
            RtfText = txt;
            db = new ApplicationContext();
            {
                CommonValues = db.CommonValues.ToArray();
                Org = db.Settings.Select(f => new Org {
                    OrgAddress = f.OrgAddress,
                    OrgEmail = f.OrgEmail,
                    OrgName = f.OrgName,
                    OrgPhone = f.OrgPhone,
                    OrgShortName = f.OrgShortName,
                    OrgSite = f.OrgSite
                }).FirstOrDefault() ?? new Org();
            }
        }

        public RtfParse(string txt, Client client, Employee employee = null) : this(txt)
        {
            Client = client;
            Employee = employee;

            if (Client != null) 
                AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == client.Id)
                    .Include(f => f.AdditionalField.TypeValue).ToArray();
          
        }

        public string Run()
        {
            try
            {
                if (Employee != null) Employee.Password = "";
                Regex regex = new Regex(@"\[(.+?)\]");

                var matches = regex.Matches(RtfText);
                if (matches.Count > 0)
                {
                    // Это для раскодирования найденого слова в кодировке rtf
                    Regex reg = new Regex(@"{\*?\\.+(;})|\s?\\[A-Za-z0-9]+|\s?{\s?\\[A-Za-z0-9]+\s?|\s?}\s?");
                    string target = "";

                    foreach (Match match in matches)
                    {
                        // раскодируем параметр и определим каким значением мы его заменяем
                        string replaceParam = reg.Replace(match.Value, target).Replace("]", "").Replace("[", "").Trim();
                        //теперь находим перезаписываем параметр в исходной строке и заменяем его значением
                        RtfText = RtfText.Replace(match.Value, GetValueProperty(replaceParam));
                    }
                }
                return RtfText;
            }
            catch
            {
                return RtfText;
            }
        }

        private string GetValueProperty(string param)
        {
            try
            {
                string propertyName = param.Substring(param.IndexOf(".") + 1).Trim();
                string modelName = param.Substring(0, param.IndexOf(".")).Trim();

                switch (modelName)
                {
                    //case "Client": return ((Client)Model)?.GetType().GetProperty(propertyName)?.GetValue(Model)?.ToString() ?? "";
                    case "Client":
                        return Client?.GetType().GetProperty(propertyName)?.GetValue(Client)?.ToString() ?? "";
                    case "Employee":
                        return Employee?.GetType()?.GetProperty(propertyName)?.GetValue(Employee)?.ToString() ?? "";
                    case "Org": 
                        return Org.GetType().GetProperty(propertyName)?.GetValue(Org)?.ToString() ?? "";

                    case "ClientAdditionalFields": return GetAdditionalClientValue(propertyName);

                    case "CommonFields":
                        return CommonValues?.Where(f => f.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";

                    default: return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private string GetAdditionalClientValue(string propertyName)
        {
            var prop = AdditionalClientValues?.Where(f => f.AdditionalField?.SysName == propertyName)?.FirstOrDefault();

            if (prop?.AdditionalField?.TypeValue?.SysName == "money")
            {
                return decimal.TryParse(prop?.Value, out decimal result) ? result.ToString("C2", CultureInfo.CurrentCulture) : prop?.Value;
            }

            if (prop?.AdditionalField?.TypeValue?.SysName == "date")
            {
                return DateTime.TryParse(prop?.Value?.ToString(), out DateTime dateTime) ? dateTime.ToShortDateString() : prop?.Value ?? "";
            }

            return prop?.Value ?? "";
        }

        public void SetEmployee()
        {
            try
            {
                Employees = db.Employes.ToArray();
                new SelectEmployee() { DataContext = this }?.ShowDialog();
            }
            catch
            {

            }
        }

        public Employee[] Employees { get; set; }
        public bool NotSetEmployee { get; set; } = false;


        private string RtfText { get; set; }
        public Employee Employee { get; set; }
        public object SelectedItem { get; set; }
        private Client Client { get; set; }
        private Org Org { get; set; }

        private ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }

        private ICollection<CommonValue> CommonValues { get; set; }

        [Command]
        public void SelectEmployee(object p)
        {
            if (NotSetEmployee == false && SelectedItem == null)
            {
                ThemedMessageBox.Show(
                    title: "Внимание",
                    text: "Этот документ содержит в шаблоне динамически подставляемые данные сотрудника. Выберите сотрудника или поставьте галочку \"Не устанавливать сотрудника\"!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Information);
                return;
            }
            if (int.TryParse(SelectedItem?.ToString(), out int result)) Employee = Employees.FirstOrDefault(f => f.Id == result);
            if (NotSetEmployee) Employee = null;
            if (p is Window win) win?.Close();
        }
    }

    public class Org
    {
        public string OrgAddress { get; set;}
        public string OrgEmail { get; set;}
        public string OrgName { get; set;}
        public string OrgPhone { get; set;}
        public string OrgShortName { get; set;}
        public string OrgSite { get; set;}
    }
}
