using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("SmsRecipients")]
    public class SmsRecipient : AbstractBaseModel
    {
        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Sms Sms { get; set; }
        public int? SmsId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public  Client Client { get; set; }
        public int? ClientId { get; set; }

        public string Contact { get; set; }

        public string Status { get; set; } //статус доставки до получателя (клиента)
    }
}
