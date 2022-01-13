using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Smsc.Settings
{
    public class SettingsException : Exception
    {
        public SettingsException(string msg) : base(msg) {}

    }
}
