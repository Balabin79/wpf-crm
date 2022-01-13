using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Smsc.Parsing
{
    public interface IParser
    {
        string Parse(string msg);
    }
}
