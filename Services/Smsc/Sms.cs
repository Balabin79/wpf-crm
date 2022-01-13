using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Smsc
{
    public class Sms : AbstractSmsCenter
    {
        public Sms(string login, string psw, ICollection<string> phones, string msg) : base(login, psw, phones)
        {
            Msg = msg;
        }

        public override void InitLogging()
        {

        }

        public override void InitSaving()
        {

        }

        public override void Logging()
        {

        }

        public override void Saving()
        {

        }

        public string Msg { get; set; }
    }
}
