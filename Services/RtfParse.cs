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
        public RtfParse(string txt)
        {
            RtfText = txt;
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
                        string replaceParam = reg.Replace(match.Value, target);
                        // предположим заменим словом Вася
                        string value = "Вася";
                        //теперь находим перезаписываем параметр в исходной строке и заменяем его значением
                        RtfText = RtfText.Replace(match.Value, value);
                    }
                }
                return RtfText;
            }
            catch (Exception e)
            {
                return RtfText;
            }
        }

        public string RtfText { get; set; }
    }
}
