using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("TemplateType")]
    public class TemplateType : AbstractBaseModel
    {
        public string CaptionRu { get; set; }
        public string CaptionEn { get; set; }
        public string SysName { get; set; }
    }
}
