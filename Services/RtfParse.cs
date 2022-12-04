using Dental.Models;
using Dental.Views.WindowForms;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services
{
    public class RtfParse : DevExpress.Mvvm.ViewModelBase
    {
        public RtfParse(string txt)
        {
            RtfText = txt;
            using (var db = new ApplicationContext())
            {
                CommonValues = db.CommonValues.ToArray();
                Organization = db.Organizations.FirstOrDefault() ?? new Organization();
            }
        }

        public RtfParse(string txt, object model) : this(txt)
        {
            if (model is Client client)
            {
                Client = client;
                using (var db = new ApplicationContext())
                    AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == client.Id).Include(f => f.AdditionalField).ToArray();
            }
            if (model is Employee employee) Employee = employee;



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
                        return Client.GetType().GetProperty(propertyName)?.GetValue(Client)?.ToString() ?? "";
                    case "Employee":
                        if (Employee == null && !NotSetEmployee) SetEmployee(); 
                        return Employee?.GetType()?.GetProperty(propertyName)?.GetValue(Employee)?.ToString() ?? "";
                    case "Org":
                        return Organization.GetType().GetProperty(propertyName)?.GetValue(Organization)?.ToString() ?? "";

                    case "ClientAdditionalFields":
                        return AdditionalClientValues?.Where(f => f.AdditionalField?.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";

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

        public void SetEmployee()
        {
            try
            {
                using (var db = new ApplicationContext())
                {
                    Employees = db.Employes.ToArray();
                }
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
        private Client Client{ get; set; }
        private Organization Organization { get; set; }

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
}
