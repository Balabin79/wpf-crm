using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services.SmsServices.ProstoSmsService
{
    [Serializable]
    public class Msg
    {
        public int err_code { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }
}
