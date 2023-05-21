using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService.BalanceMethod
{
    [Serializable]
    class Balance
    {
        public BalanceResponse response { get; set; }
    }
}
