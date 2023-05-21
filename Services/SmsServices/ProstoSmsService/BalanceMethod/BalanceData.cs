using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService.BalanceMethod
{
    [Serializable]
    public class BalanceData
    {
        public int id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public decimal? credits { get; set; }
        public decimal? credits_used { get; set; }
        public string sender_name { get; set; }
    }
}
