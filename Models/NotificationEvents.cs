using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
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

        public string TelegramToken
        {
            get { return GetProperty(() => TelegramToken); }
            set { SetProperty(() => TelegramToken, value); }
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
