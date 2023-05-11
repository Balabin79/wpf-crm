using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("Providers")]
    public class Provider : AbstractBaseModel
    {
        public string Name { get; set; }
    }
}
