using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Dental.Models.Base;
using Dental.Services;
using System.Data.SQLite;


namespace Dental.Models
{
    [DbConfigurationType(typeof(NpgSqlConfiguration))]
    public class PostgresContext : ApplicationContext
    {
        public PostgresContext(string conn) : base(conn){}
    }
}
