using Dental.Models.Base;
using DevExpress.CodeParser;
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

        public string TelegramToken { get; set; }
        public bool? IsNotifyByTelegram { get; set; }    
    }
 
}
