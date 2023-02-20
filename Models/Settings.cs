using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Setting : AbstractBaseModel
    {
        public string LoginProviderMsg { get; set; }
        public string PasswordProviderMsg { get; set; }

        public int? ProviderMsgId { get; set; }
        public ProviderMsg ProviderMsg { get; set; }


        public string OrgName { get; set; }
        public string OrgShortName { get; set; }
        public string OrgAddress { get; set; }
        public string OrgEmail { get; set; }
        public string OrgPhone { get; set; }
        public string OrgSite { get; set; }


    }
}
