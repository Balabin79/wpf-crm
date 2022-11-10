using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models
{
    [Serializable]
    [Table("TemplateType")]
    public class TemplateType : AbstractBaseModel
    {
        public string CaptionRu { get; set; }
        public string CaptionEn { get; set; }
        public string SysName { get; set; }
    }
}
