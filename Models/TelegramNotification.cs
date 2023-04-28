using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("TelegramNotifications")]
    public class TelegramNotification : AbstractBaseModel
    {
        public string ChatId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public NotificationEvent NotificationEvent { get; set; }

        public int? NotificationEventId { get; set; }

        public string Msg { get; set; }
        public string DateRelevance { get; set; }
    }
}
