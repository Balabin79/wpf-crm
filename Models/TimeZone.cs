using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.Models
{
    [Table("TimeZones")]
    public class TimeZone : AbstractBaseModel
    {        
        public string Name{get; set;}
        public int? TZ { get; set; }
        public int? UTC { get; set; }

        [NotMapped]
        public string FullName 
        {
            get => TZ < 0 
                ? Name + " (" + TZ + " ч. от Мск)"
                :TZ == 0
                ? Name
                : Name + " (+" + TZ + " ч. от Мск)";
        }
    }
}