using Dental.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dental.Services
{
    public static class ActionsLog
    {
        private static readonly ApplicationContext db;

        static string sessionGuid;

        static ActionsLog()
        {
            db = new ApplicationContext();
        }

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
                db.UserActions.Add(
                    new UserActions() {
                        Name = name, 
                        Type = type, 
                        UserName = Environment.UserName,
                        SectionPage = sectionPage,
                        SessionGuid = SessionGuid
                    }
                );
                db.SaveChanges();
            } 
            catch
            { 
            
            }
        }

        public static void RegisterAction(string[] name, string type, string sectionPage)
        {
            try
            {
                string path = (name.Length > 1) ? string.Join("->", name) : name[0];
                db.UserActions.Add(
                    new UserActions() { 
                        Name = path, 
                        Type = type, 
                        UserName = Environment.UserName,
                        SectionPage = sectionPage
                    }
                );
                db.SaveChanges();
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
            { "Speciality", "Специальности сотрудников" },
            { "Classificator", "Классификатор услуг" },
            { "EmployeeGroup", "Категории сотрудников" },
            { "ClientGroup", "Категории клиентов" },
            { "ClientsRequests", "Список обращений" },
            { "ClientsSubscribes", "Рассылки" },
            { "Organization", "Организация" },
            { "SmsCenter", "Настройки SmsCenter" },
            { "StatusSheduler", "Статусы в расписании" },

            { "ClientInfo", "Карта клиента" },
            { "Plan", "План услуг" },
            { "PlanItem", "Позиция в план услуг" },
            { "Ids", "ИДС" },

            { "Employee", "Карта сотрудника" },
            { "", "" },
        };

        public static void ClearHistoryActions()
        {
            try
            {

                int currentTimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970,1,1)).TotalSeconds;
                int delta = currentTimeStamp - 10 * 24 * 60 * 60;
                var q = db.UserActions.Where(f => f.CreatedAt < delta).ToArray();
                db.UserActions.RemoveRange(q);
                db.SaveChanges();
            }
            catch
            {

            }
        }

    }
}
