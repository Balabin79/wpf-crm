using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("SendingStatuses")]
    public class SendingStatus : AbstractBaseModel
    {
        public string Name { get; set; }
    }
}
