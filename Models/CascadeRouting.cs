using B6CRM.Models.Base;
using DevExpress.CodeParser;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("CascadeRouting")]
    public class CascadeRouting : AbstractBaseModel
    {
        public string Channel { get; set; }
        public string Abbr { get; set; }

        public int? ProviderId { get; set; } = 1;

        public int? IsActive { get; set; }

        public int? Num { get; set; } = 0;
    }

}
