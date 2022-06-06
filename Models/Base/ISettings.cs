using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public interface ISettings
    {
        string Key { get; set; }
        string Value { get; set; }
    }
}
