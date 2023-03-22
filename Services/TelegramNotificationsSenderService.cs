using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dental.Models;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Dental.Services
{
    internal static class TelegramNotificationsSenderService
    {
        static ApplicationContext db;

        static TelegramNotificationsSenderService() => db = new ApplicationContext();

        public static async void Send()
        {
            try
            {
                var msgs = db.TelegramNotifications.Include(f => f.NotificationEvent).ToArray();

                foreach (var msg in msgs)
                {
                    var result = await SendMsg(msg);
                    if(result) db.TelegramNotifications.Remove(msg);
                }
                db.SaveChanges();
            }
            catch(Exception e)
            { 
                Log.ErrorHandler(e);
                db.SaveChanges();
            }           
        }

        public static async void Send(TelegramNotification telegramNotification) => await SendMsg(telegramNotification);

        private static async Task<bool> SendMsg(TelegramNotification telegramNotification)
        {
            try
            {
                //проверяем актуальность сообщения (для них установлено значение DateRelevance)
                // если сообщение устарело, то просто удаляем
                if (!DateTime.TryParse(telegramNotification.DateRelevance, out DateTime dateRelevance) || dateRelevance < DateTime.Now) 
                {
                    return true;
                }

                var botClient = new TelegramBotClient(telegramNotification?.NotificationEvent?.TelegramToken);
                using CancellationTokenSource cts = new();

                var message = await botClient.SendTextMessageAsync(
                chatId: new ChatId(telegramNotification?.ChatId),
                text: telegramNotification?.Msg,
                cancellationToken: cts.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
