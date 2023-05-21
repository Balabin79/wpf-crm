using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService.BalanceMethod
{
    [Serializable]
    public class BalanceResponse
    {
        public Msg msg { get; set; }
        public BalanceData data { get; set; }
    }
}
