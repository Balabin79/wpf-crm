using Dental.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dental.Services
{
    public class HtmlParse
    {
        private readonly ApplicationContext db; 

        public HtmlParse(string txt)
        {
            db = new ApplicationContext();
            HtmlText = txt;
            CommonValues = db.CommonValues.ToArray();
            Organization = db.Organizations.FirstOrDefault() ?? new Organization();
        }

        public HtmlParse(string txt, object model) : this(txt) 
        { 
            if (model is Client client)
            {
                Client = client;
                AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == client.Id).Include(f => f.AdditionalField).ToArray();
            }
            if (model is Employee employee) 
            {
                Employee = employee;
                AdditionalEmployeeValues = db.AdditionalEmployeeValue.Where(f => f.EmployeeId == employee.Id).Include(f => f.AdditionalField).ToArray();
            }

            if (model is Invoice invoice)
            {
                Invoice = invoice;
                InvoiceServiceItems = db.InvoiceServiceItems.Where(f => f.InvoiceId == invoice.Id).
                    Include(f => f.Employee).Include(f => f.Service).ToArray();
                InvoiceMaterialItems = db.InvoiceMaterialItems.Include(f => f.Nomenclature).ToArray();

                // строку выбирать из бд динамически непосредственно при парсинге
                //AdditionalEmployeeValues = db.AdditionalEmployeeValue.Where(f => f.EmployeeId == employee.Id).Include(f => f.AdditionalField).ToArray();
                if (invoice.ClientId != null )
                    AdditionalClientValues = db.AdditionalClientValue.Where(f => f.ClientId == invoice.ClientId).
                        Include(f => f.AdditionalField).ToArray();
            }
        }

        public string Run()
        {
            try
            {
                if (Employee != null) Employee.Password = "";
                Regex regex = new Regex(@"\[(.+?)\]");

                var matches = regex.Matches(HtmlText);
                if (matches.Count > 0)
                {
                    // Это для раскодирования найденого слова в кодировке rtf
                    Regex reg = new Regex(@"{\*?\\.+(;})|\s?\\[A-Za-z0-9]+|\s?{\s?\\[A-Za-z0-9]+\s?|\s?}\s?");
                    string target = "";

                    foreach (Match match in matches)
                    {
                        // раскодируем параметр и определим каким значением мы его заменяем
                        string replaceParam = reg.Replace(match.Value, target).Replace("]","").Replace("[", "").Trim();
                        //теперь находим перезаписываем параметр в исходной строке и заменяем его значением
                        HtmlText = HtmlText.Replace(match.Value, GetValueProperty(replaceParam));
                    }
                }
                return HtmlText;
            }
            catch
            {
                return HtmlText;
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
                    case "Client": return Client.GetType().GetProperty(propertyName)?.GetValue(Client)?.ToString() ?? "";
                    case "Employee": return Employee.GetType().GetProperty(propertyName)?.GetValue(Employee)?.ToString() ?? "";
                    case "Org": return Organization.GetType().GetProperty(propertyName)?.GetValue(Organization)?.ToString() ?? "";
                        
                    case "ClientAdditionalFields": return AdditionalClientValues?.Where(f => f.AdditionalField?.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";
                    case "EmployeeAdditionalFields": return AdditionalEmployeeValues?.Where(f => f.AdditionalField?.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";
                    case "CommonFields": return CommonValues?.Where(f => f.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";

                    default: return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }        
        }


        private string HtmlText { get; set; }
        private Employee Employee { get; set; }
        private Client Client{ get; set; }
        private Organization Organization { get; set; }

        private ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }
        private ICollection<AdditionalEmployeeValue> AdditionalEmployeeValues { get; set; }
        private ICollection<CommonValue> CommonValues { get; set; }

        private Invoice Invoice { get; set; }
        private ICollection<InvoiceServiceItems> InvoiceServiceItems { get; set; }
        private ICollection<InvoiceMaterialItems> InvoiceMaterialItems { get; set; }
    }
}
