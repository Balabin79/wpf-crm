using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("TemplateType")]
    public class TemplateType : AbstractBaseModel
    {
        public string CaptionRu { get; set; }
        public string CaptionEn { get; set; }
        public string SysName { get; set; }
    }
}
