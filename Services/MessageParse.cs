using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dental.Services
{
    public class MessageParse
    {
        public MessageParse(string txt, Client[] clients)
        {
            ApplicationContext db = new ApplicationContext();
            Text = txt;
            Clients = clients;
            Org = db.Organizations.FirstOrDefault();
            Msgs = new List<string>();
        }

        public List<string> Run()
        {
            try
            {
                Regex regex = new Regex(@"\[(.+?)\]");

                var matches = regex.Matches(Text);


                foreach (var i in Clients)
                {
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            // раскодируем параметр и определим каким значением мы его заменяем
                            string replaceParam = match.ToString().Replace("]", "").Replace("[", "").Trim();
                            //теперь находим перезаписываем параметр в исходной строке и заменяем его значением
                            Msgs.Add(Text.Replace(match.ToString(), GetValueProperty(replaceParam, i)));
                        }
                    }
                }

                return Msgs;
            }
            catch
            {
                return Msgs;
            }
        }

        private string GetValueProperty(string param, Client client) 
        {
            try
            {
                string propertyName = param.Substring(param.IndexOf(".") + 1).Trim();
                string modelName = param.Substring(0, param.IndexOf(".")).Trim();

                switch (modelName)
                {
                    case "Org": return Org?.GetType().GetProperty(propertyName)?.GetValue(Org)?.ToString() ?? "";
                    case "Client": return Clients[0]?.GetType().GetProperty(propertyName)?.GetValue(client)?.ToString() ?? "";
                    default: return "";
                }
            }
            catch
            {
                return "";
            }
        
        }           

        private string Text { get; set; }
        private Client[] Clients { get; set; }
        private List<string> Msgs { get; set; }
        private Organization Org { get; set; }
    }
}
