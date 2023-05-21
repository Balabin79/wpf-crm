using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService.Response.BalanceMethod
{
    [Serializable]
    class Balance
    {
        public BalanceResponse response { get; set; }
    }
}
