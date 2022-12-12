using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services
{
    [Serializable]
    public class UserConfig
    {
        public string DBName { get; set; }
        public string ConnectionString { get; set; }
        public string PathToProgram { get; set; }
    }
}
