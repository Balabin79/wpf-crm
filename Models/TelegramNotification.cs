using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("TelegramNotifications")]
    public class TelegramNotification : AbstractBaseModel
    {
        public string ChatId { get; set; }

        public NotificationEvent NotificationEvent { get; set; }
        public int? NotificationEventId { get; set; }

        public string Msg { get; set; }
    }
}
