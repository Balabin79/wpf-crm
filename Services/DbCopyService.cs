using B6CRM.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Services
{
    internal class DbCopyService : ViewModelBase
    {
        private ApplicationContext SQLiteContext { get; set; }
        private ApplicationContext PostgreSQLContext { get; set; }

        internal DbCopyService(ApplicationContext db)
        {
            try
            {
                SQLiteContext = new ApplicationContext(new Config() { DbType = 0, ConnectionString = Config.PathToDbDefault });
                PostgreSQLContext = db;
                if (PostgreSQLContext.Config?.DbType == 0)
                {
                    throw new Exception("Для копирования данных, необходимо установить PostgreSQL в качестве ведущей базы данных!");
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, e.Message, true);
            }
        }

        [Command]
        public void DataCopyFromSQLiteToPostgreSQL()
        {

        }

        [Command]
        public void DataCopyFromPostgreSQLToSQLite()
        {

        }

        private void Copy()
        {

        }
    }
}
