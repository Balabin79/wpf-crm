﻿using Dental.Models;
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
        public RtfParse(string txt, Client model)
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
                    case "Client": return Model?.GetType().GetProperty(propertyName)?.GetValue(Model)?.ToString() ?? "";
                    default: return "";
                }
            }
            catch
            {
                return "";
            }
        
        }           

        private string RtfText { get; set; }
        private Client Model { get; set; }
    }
}
