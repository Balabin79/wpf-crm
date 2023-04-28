using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("NotificationEvents")]
    public class NotificationEvent : AbstractBaseModel
    {
        public NotificationEvent() => IsNotify = 0;

        public string EventName
        {
            get { return GetProperty(() => EventName); }
            set { SetProperty(() => EventName, value); }
        }

        public int? TelegramBotId
        {
            get { return GetProperty(() => TelegramBotId); }
            set { SetProperty(() => TelegramBotId, value); }
        }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public TelegramBot TelegramBot
        {
            get { return GetProperty(() => TelegramBot); }
            set { SetProperty(() => TelegramBot, value); }
        }

        public int? IsNotify
        {
            get { return GetProperty(() => IsNotify); }
            set { SetProperty(() => IsNotify, value); }
        }

        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }
    }
}
