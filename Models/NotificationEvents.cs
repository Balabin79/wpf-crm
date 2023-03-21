using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("NotificationEvents")]
    public class NotificationEvent : AbstractBaseModel
    {
        public string Name { get; set; }
        public string ChatId { get; set; }
    }
}
