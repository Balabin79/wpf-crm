using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Setting : AbstractBaseModel
    {
        public string OrgName { get; set; }
        public string OrgShortName { get; set; }
        public string OrgAddress { get; set; }
        public string OrgEmail { get; set; }
        public string OrgPhone { get; set; }
        public string OrgSite { get; set; }

        public int? DbType { get; set; } = (int)DbList.SQLite;

        [NotMapped]
        public PostgresConnect PostgresConnect { get; set; }
    }

    public enum DbList { SQLite = 0, PostgreSQL = 1 }
}
