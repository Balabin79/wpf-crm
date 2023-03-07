using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    [Serializable]
    public class StoreConnectToDb
    {
        public string ConnectionString { get; set; }
        public int Db { get; set; }
    }
}
