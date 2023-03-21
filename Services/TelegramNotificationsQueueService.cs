using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dental.Models;

namespace Dental.Services
{
    internal static class TelegramNotificationsQueueService
    {
        public static void AddToQueue(TelegramNotification notification)
        {
            using(var db = new ApplicationContext())
            {
                db.TelegramNotifications.Add(notification);
                db.SaveChanges();
            }
        }
    }
}
