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
    public class RtfParse
    {
        private readonly ApplicationContext db; 

        public RtfParse(string txt)
        {
            db = new ApplicationContext();
            RtfText = txt;
            CommonValues = db.CommonValues.ToArray();
        }

        public RtfParse(string txt, object model) : this(txt) 
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
        }

        public string Run()
        {
            try
            {
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
                        string replaceParam = reg.Replace(match.Value, target).Replace("]","").Replace("[", "").Trim();
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
                    case "Client": return Client.GetType().GetProperty(propertyName)?.GetValue(Client)?.ToString() ?? "";
                    case "Employee": return Employee.GetType().GetProperty(propertyName)?.GetValue(Employee)?.ToString() ?? "";        
                    case "ClientAdditionalValues": return AdditionalClientValues?.Where(f => f.AdditionalField?.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";
                    case "EmployeeAdditionalValues": return AdditionalEmployeeValues?.Where(f => f.AdditionalField?.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";
                    case "CommonValues": return CommonValues?.Where(f => f.SysName == propertyName)?.FirstOrDefault()?.Value ?? "";

                    default: return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }        
        }

        // case "Employee": return ((Employee)Model)?.GetType().GetProperty(propertyName)?.GetValue(Model)?.ToString() ?? "";
        private string RtfText { get; set; }
        private Employee Employee { get; set; }
        private Client Client{ get; set; }
        private ICollection<AdditionalClientValue> AdditionalClientValues { get; set; }
        private ICollection<AdditionalEmployeeValue> AdditionalEmployeeValues { get; set; }
        private ICollection<CommonValue> CommonValues { get; set; }
    }
}
