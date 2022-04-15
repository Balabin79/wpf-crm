using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dental.Services
{
    public static class ActionsLog
    {
        static string sessionGuid;
        static string SessionGuid 
        { 
            get
            {
                if (sessionGuid == null) sessionGuid = KeyGenerator.GetUniqueKey(20);
                return sessionGuid;
            } 
        }

        public static void RegisterAction(string name, string type, string sectionPage)
        {
            try 
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.UserActions.Add(
                        new UserActions()
                        {
                            Name = name,
                            Type = type,
                            UserName = Environment.UserName,
                            SectionPage = sectionPage,
                            SessionGuid = SessionGuid
                        });
                    db.SaveChanges();
                }
            } 
            catch
            { 
            
            }
        }

        public static void RegisterAction(string[] name, string type, string sectionPage)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    string path = (name.Length > 1) ? string.Join("->", name) : name[0];
                    db.UserActions.Add(
                        new UserActions()
                        {
                            Name = path,
                            Type = type,
                            UserName = Environment.UserName,
                            SectionPage = sectionPage
                        }
                    );
                    db.SaveChanges();
                }
            }
            catch
            {

            }
        }

        public static Dictionary<string, string> ActionsRu { get; } = new Dictionary<string, string>() {
            { "add", "создание" },
            { "edit", "редактирование" },
            { "delete", "удаление" }
        };

        public static Dictionary<string, string> SectionPage { get; } = new Dictionary<string, string>() {
            { "Advertising", "Рекламные источники" },
            { "Measure", "Единицы измерения" },
            { "Warehouse", "Склады" },
            { "ServicesItems", "Перечень услуг" },
            { "NomenclatureItems", "Перечень товаров" },
            { "ClientsRequests", "Список обращений" },
            { "ClientsSubscribes", "Рассылки" },
            { "SmsCenter", "Настройки SmsCenter" },
            { "StatusSheduler", "Статусы в расписании" },
            { "ClientInfo", "Карта клиента" },
            { "Ids", "ИДС" },
            { "Employee", "Карта сотрудника" },
            { "", "" },
        };

        public static void ClearHistoryActions()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    int currentTimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    int delta = currentTimeStamp - 10 * 24 * 60 * 60;
                    var q = db.UserActions.Where(f => f.CreatedAt < delta).ToArray();
                    db.UserActions.RemoveRange(q);
                    db.SaveChanges();
                }
            }
            catch
            {

            }
        }

    }
}
