using System;

namespace Dental.Models
{
    public class Organization
    {
        //Общая инф-ция
        public string Inn { get; set; } // Инн
        public string Kpp { get; set; } // Кпп
        public string Name { get; set; } // Наименование орг-ции
        public string ShortName { get; set; } // Сокращение
        
        // Контактная инф-ция
        public string Address { get; set; } // Адрес
        public string Phone { get; set; }
        public string Email { get; set; }

        // Банковские реквизиты
        public string Bik { get; set; } // Бик
        public string AccountNumber { get; set; } // Расчетный счет
        public string BankName { get; set; } // Наименование

        // Регистрационная информация
        public string Сertificate { get; set; } // Свидетельство
        public string Ogrn { get; set; } // ОГРН                                      
        public string GeneralDirector { get; set; } // Генеральный директор
        public string License { get; set; } // Лицензия
        public string WhoIssuedBy { get; set; } // Кем выдана

        // Служебные поля
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
