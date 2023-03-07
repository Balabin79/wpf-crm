using Dental.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.Base
{
    public class ConnectToDb
    {
        public ConnectToDb()
        {
            Config = new Config();
            if (Config.DbType == 1) Context = new PostgresContext(Config.ConnectionString); 
            
            else Context = new SQLiteContext(Config.ConnectionString);
        }

        public Config Config { get; private set; }
        public ApplicationContext Context { get; private set; }
    }
}
