using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using Dental.Services;
using System.Data.SQLite;


namespace Dental.Models
{
    public class SQLiteContext : ApplicationContext
    {

        public SQLiteContext(string conn) : base(
            new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder()
                {
                    DataSource = conn,
                    Version = 3,
                    BusyTimeout = 10000,
                    FailIfMissing = true
                }.ConnectionString
            }, true
            ){}
    }
}
