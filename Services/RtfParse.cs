using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dental.Services
{
    public class RtfParse
    {
        public RtfParse(string txt, PatientInfo model)
        {
            RtfText = txt;
            Model = model;
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
            catch (Exception e)
            {
                return RtfText;
            }
        }

        private string GetValueProperty(string nameProperty) => Model.GetType().GetProperty(nameProperty)?.GetValue(Model).ToString();            

        private string RtfText { get; set; }
        private PatientInfo Model { get; set; }
    }
}
