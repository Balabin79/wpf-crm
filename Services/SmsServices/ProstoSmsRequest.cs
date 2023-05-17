using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices
{
    [Serializable]
    public class ProstoSmsRequest
    {
        public string method { get; set; }
        public string format { get; set; } = "json";
        public string email { get; set; }
        public string password { get; set; }
        public string key { get; set; }
        public string text { get; set; }
        public string phone  { get; set; }
        public string sender_name  { get; set; }
        public string external_id { get; set; }
        public int priority { get; set; }
    }
}
