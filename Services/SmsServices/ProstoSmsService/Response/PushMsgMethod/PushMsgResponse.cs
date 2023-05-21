using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService.Response.PushMsgMethod
{
    [Serializable]
    public class PushMsgResponse
    {
        public Msg msg { get; set; }
        public PushMsgData data { get; set; }
    }
}
