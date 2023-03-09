using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;

namespace Dental.Models.Base
{
    internal class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {/*
            SetProviderFactory("Microsoft.Data.Sqlite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));*/
        }
    }
}
