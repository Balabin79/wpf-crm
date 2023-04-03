using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B6CRM.Models;

namespace B6CRM.Services
{
    internal static class TelegramNotificationsQueueService
    {
        public static void AddToQueue(TelegramNotification notification, ApplicationContext db)
        {
            try
            {
                // если есть сеть, то сразу отправить, если нет, то записать в бд
                if (PingService.IsNetworkAvailable())
                {
                    TelegramNotificationsSenderService.Send(notification);
                    return;
                }

                db.TelegramNotifications.Add(notification);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
            }

        }
    }
}
