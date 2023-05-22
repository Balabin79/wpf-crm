using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.SmsSenders
{
    public class HttpClientInstance
    {
        private static readonly HttpClient instance;
        static HttpClientInstance() => instance = new HttpClient();
        
        public static HttpClient getInstance() => instance != null ? instance : new HttpClient();      
    }
}
