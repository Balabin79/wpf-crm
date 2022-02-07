using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services
{
    public static class Helper
    {
        public static ICollection<string> Pages
        {
            get => new string[]
            {
                "Расписание",
                "Список клиентов",
                "Список обращений",
                "Рассылки",
                "Сводные данные",
                "Планы услуг и продажи",
                "Счета и оплаты",
                "Выручка за периоды",
                "Приемы клиентов",
                "Расход материалов",
                "Каналы привлечения",
                "Список сотрудников",
                "Категории клиентов",
                "Категории сотрудников",
                "Рекламные источники",
                "Реквизиты организации",
                "Специальности сотрудников",
                "Классификатор услуг",
                "Настройки"
            };
        }

    }
}
