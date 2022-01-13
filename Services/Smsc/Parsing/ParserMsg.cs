using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Smsc.Parsing
{
    class ParserMsg : IParser
    {
        public string Parse(string msg)
        {
            return msg;
            // throw new ParsingMsgException(5);
        }
    }
}
