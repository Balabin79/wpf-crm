using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ProviderMsgs")]
    public class ProviderMsg : AbstractBaseModel
    {
        public string Name { get; set; }
        public string ProviderUrl { get; set; }

        public Country Country { get; set; }
        public int? CountryId{ get; set; }
    }
}
