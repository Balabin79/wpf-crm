using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Smsc.Parsing
{
    class ParsingMsgException : Exception
    {
        public ParsingMsgException(string msg = "Невозможно провести подстановку значений в отправляемое сообщение", int pos = 0) : base(msg) 
        {
            Message = msg;
            if (pos > 0) Message += ", ошибка в строке, в позиции " + pos; 

        }
        public override string Message { get; }
    }
}
