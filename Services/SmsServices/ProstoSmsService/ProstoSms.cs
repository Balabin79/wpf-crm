using B6CRM.Models;
using B6CRM.Services.SmsServices.ProstoSmsService;
using B6CRM.Services.SmsServices.SmscService;
using B6CRM.ViewModels.SmsSenders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices
{
    public class ProstoSms
    {
        private readonly string login;
        private readonly string password;
        private readonly string apiUrl = "https://ssl.bs00.ru";
        private readonly string senderName;
        private readonly bool isCascadeRouting;

        private readonly HttpClient httpClient;

        public ProstoSms(
            ServicePassViewModel servicePassVm
            )
        {
            this.login = servicePassVm?.ServicePass?.Login?.Trim();
            this.password = servicePassVm?.ServicePass?.PassDecr?.Trim();
            this.senderName = servicePassVm?.ServicePass?.SenderName?.Trim();
            this.isCascadeRouting = servicePassVm?.ServicePass?.IsCascadeRoutingEnabled == 1;
            /*if (apiKey != null)
                this.apiKey = apiKey;*/

            httpClient = HttpClientInstance.getInstance();
        }

        public async Task<HttpResponseMessage> SendMsg(string contacts, Sms sms)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["method"] = "push_msg",
                ["email"] = login,
                ["password"] = password,
                ["text"] = sms?.Msg ?? "",
                ["phone"] = contacts,
                ["format"] = "json"
            };
            if (!string.IsNullOrWhiteSpace(this.senderName)) data["sender_name"] = senderName;

            // если установлена дата отложенной доставки
            if (!string.IsNullOrWhiteSpace(sms?.Date) && DateTime.TryParse(sms?.Date, out DateTime date2))
            {
                data["set_aside_time"] = new DateTimeOffset(date2).ToUnixTimeSeconds().ToString();
                data["time"] = "local";
            }

            data["route"] = GetRouting(sms);



            // создаем объект HttpContent
            HttpContent contentForm = new FormUrlEncodedContent(data);
            // отправляем запрос
            var response = await httpClient.PostAsync(new Uri(apiUrl), contentForm);
            return response;
            //return new HttpResponseMessage();
        }


        //Получение информации об остатке баланса(метод get_profile)
        public async Task<HttpResponseMessage> GetAccountBalance()
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["method"] = "get_profile",
                ["email"] = login,
                ["password"] = password,
                ["format"] = "json"
            };

            // создаем объект HttpContent
            HttpContent contentForm = new FormUrlEncodedContent(data);
            // отправляем запрос
            var response = await httpClient.PostAsync(new Uri(apiUrl), contentForm);
            return response;
        }

        //Получение статусов самостоятельно (метод get_msg_report)
        public async Task<HttpResponseMessage> GetMsgReport(int smsId)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["method"] = "get_msg_report",
                ["email"] = login,
                ["password"] = password,
                ["format"] = "json"
            };

            // создаем объект HttpContent
            HttpContent contentForm = new FormUrlEncodedContent(data);
            // отправляем запрос
            var response = await httpClient.PostAsync(new Uri(apiUrl), contentForm);
            return response;
        }

        private string GetRouting(Sms sms)
        {
            using (var db = new ApplicationContext())
            {
                if (isCascadeRouting)
                {
                    var route = db.CascadeRouting?.Where(f => f.ProviderId == 1 && f.IsActive == 1)?.OrderBy(f => f.Num)?.Select(f => f.Abbr)?.ToArray() ?? new string[0];

                    var param = string.Join("-", route);
                    if (!string.IsNullOrEmpty(param)) return param;
                }
                switch (sms?.Channel?.Name)
                {
                    case "Sms": return "sms";
                    case "Telegram": return "tg";
                    case "Viber": return "vb";
                    case "WhatsApp": return "wp";
                    case "VK": return "vk";

                    default: return "sms";
                }
            }
        }

    }
}
