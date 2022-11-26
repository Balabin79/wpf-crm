using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.SqlClient;

namespace Dental.Models
{
   /* public class DbConf : DbConfiguration, IDbConnectionFactory
    {
        public SqlTypeEnum sqlTypeEnum = SqlTypeEnum.Sqlite;

       // public DbConf()
       // {
        //    SetDefaultConnectionFactory(this);
        //}

        public DbConf()
        {
            //this.sqlTypeEnum = sqlTypeEnum;
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);

            var providerServices = (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices));

            SetProviderServices("System.Data.SQLite", providerServices);
            SetProviderServices("System.Data.SQLite.EF6", providerServices);
            SetDefaultConnectionFactory(this);
        }

        public DbConnection CreateConnection(string connection)
        {
            //if (sqlTypeEnum == SqlTypeEnum.Postgres)
                return new SqlConnection("Data Source=.\\dental.db");
            //else
                //return new SQLiteConnection(connectionString);
        }
    }

    public enum SqlTypeEnum { Sqlite = 0, Postgres = 1};*/
}
