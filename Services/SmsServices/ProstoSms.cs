using B6CRM.Models;
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
    public class ProstoSms : ISmsSending
    {
        private readonly string login;
        private readonly string password;
        private readonly string text;
        private readonly string apiUrl = "https://ssl.bs00.ru";
        private readonly string apiKey;
        private readonly string contacts;
        private readonly string senderName;

        private readonly HttpClient httpClient;

        public ProstoSms(
            ServicePassViewModel servicePassVm,
            string contacts,
            Sms sms
            )
        {
            this.login = servicePassVm?.ServicePass?.Login?.Trim();
            this.password = servicePassVm?.ServicePass?.PassDecr?.Trim();
            this.senderName = servicePassVm?.ServicePass?.SenderName?.Trim();

            this.text = sms.Msg;
            this.contacts = contacts;

            /*if (apiKey != null)
                this.apiKey = apiKey;*/

            httpClient = new HttpClient() { BaseAddress = new Uri(this.apiUrl) };
        }

        public async Task<HttpResponseMessage> SendMsg()
        {
            string request = $"{apiUrl}/?method=push_msg&email={login}&password={password}&text={text}&phone={contacts}&format=json";

            if (!string.IsNullOrWhiteSpace(this.senderName)) request += $"&sender_name={senderName}";
            

            // отправляем запрос
            var response = await httpClient.GetAsync(request);
            return response;
        }
      

        //Получение информации об остатке баланса(метод get_profile)
        public async Task<HttpResponseMessage> GetAccountBalance()
        {
            string request = $"{apiUrl}/?method=get_profile&email={login}&password={password}&format=json";

            // отправляем запрос
            var response = await httpClient.GetAsync(request);
            return response;
        }

        //Получение статусов самостоятельно (метод get_msg_report)
        public async Task<HttpResponseMessage> GetMsgReport(int smsId)
        {
            string request = $"{apiUrl}/?method=get_msg_report&email={login}&password={password}&format=json&id={smsId}";

            // отправляем запрос
            var response = await httpClient.GetAsync(request);
            return response;
        }

        //Получение информации по стоимости СМС(метод get_prices)
        public async Task<HttpResponseMessage>  GetPrice()
        {
            string request = $"{apiUrl}/?method=get_prices&email={login}&password={password}&format=json";

            // отправляем запрос
            var response = await httpClient.GetAsync(request);
            return response;
        }

        private string GetChannelParam(string channelName)
        {
            switch(channelName) 
            {
                case "sms": return "sms";
                default: return "sms";
            }
        }
    }
}
