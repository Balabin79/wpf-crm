using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B6CRM.Services.SmsServices.ProstoSmsService.Response;

namespace B6CRM.Services.SmsServices.ProstoSmsService.Response.BalanceMethod
{
    [Serializable]
    public class BalanceResponse
    {
        public Msg msg { get; set; }
        public BalanceData data { get; set; }
    }
}
