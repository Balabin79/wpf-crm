using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService.Response.PushMsgMethod
{
    [Serializable]
    public class PushMsgData
    {
        public int id { get; set; }
        public decimal? credits { get; set; }
        public int? n_raw_sms { get; set; }
        public string sender_name { get; set; }
    }
}
