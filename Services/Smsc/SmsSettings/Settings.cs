﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dental.Services.Smsc.SmsSettings
{
    /// <summary>
    /// Второй в иерархии класс, содержит большинство настроек параметров к сервису.
    /// Параметры являются общими для всех типов рассылок (смс, емайл и т.д.)
    /// 
    /// Конструктор инициализируется словарем params, ключи - имена передаваемых параметров
    /// </summary>

    [Serializable]
    sealed public class Settings
    {

        /*	
        * Имя отправителя, отображаемое в телефоне получателя. Разрешены английские буквы, цифры, пробел и
        * некоторые символы. Длина – 11 символов или 15 цифр. Все имена регистрируются в личном кабинете
        */
        public string Sender { get; set; }


        /*
         * Признак того, что сообщение необходимо перевести в транслит.
         *  0 (по умолчанию) – не переводить в транслит.
         *  1 – перевести в транслит в виде "translit".
         *  2 – перевести в транслит в виде "mpaHc/Ium".
         */
        public int Translit { get; set; } = 0;


        /*
         * Автоматически сокращать ссылки в сообщениях. Позволяет заменять ссылки в тексте сообщения на
         *  короткие для сокращения длины, а также для отслеживания количества переходов в лк
         * 0 (по умолчанию) – оставить ссылки в тексте сообщения без изменений.
         * 1 – сократить ссылки.
         */
        public int Tinyurl { get; set; } = 0;


        /*
         * Время отправки SMS-сообщения абоненту.
         * Форматы:
         * DDMMYYhhmm или DD.MM.YY hh:mm.
         * Если time = 0 или указано уже прошедшее время, то сообщение будет отправлено немедленно.
         */
        public DateTime Time { get; set; }


        /*
         * Часовой пояс, в котором задается параметр time. Указывается относительно московского времени.
         * Параметр tz может быть как положительным, так и отрицательным. Если tz равен 0, то будет  
         * использован московский часовой пояс, если же параметр tz не задан, то часовой пояс будет взят из
         * настроек Клиента.
         */
        public int Tz { get; set; } = 0;


        /*
         * Промежуток времени, в течение которого необходимо отправить рассылку. Представляет собой число  диапазоне от 0.1 до 720 часов. Применяется совместно с параметром freq. Данный параметр позволяет растянуть рассылку во времени для постепенного получения SMS-сообщений абонентами.
         */
        public float Period { get; set; }


        /*
         * Интервал или частота, с которой нужно отправлять SMS-рассылку на очередную группу номеров. Количество номеров в группе рассчитывается автоматически на основе параметров period и freq. Задается в промежутке от 1 до 1440 минут. Без параметра period параметр freq игнорируется.
         */
        public int Freq { get; set; }


        /*
         * Признак Flash сообщения, отображаемого сразу на экране телефона.
            0 (по умолчанию) – обычное сообщение.
            1 – Flash сообщение.
         */
        public int Flash { get; set; } = 0;


        /*
         * Признак wap-push сообщения, с помощью которого можно отправить интернет-ссылку на телефон.
           0 (по умолчанию) – обычное сообщение.
           1 – wap-push сообщение. В параметре mes необходимо передать ссылку и заголовок через перевод строки.
         */
        public int Push { get; set; } = 0;
        
        public string TitleWap { get; set; } = "";


        /*
         * Признак HLR-запроса для получения информации о номере из базы оператора без отправки реального SMS.
            0 (по умолчанию) – обычное сообщение.
            1 – HLR-запрос. Будет выполнен HLR-запрос для каждого номера телефона в списке. Параметр mes не используется.
         */
        public int Hlr { get; set; } = 0;


        /*
         Признак специального SMS, не отображаемого в телефоне, для проверки номеров на доступность в реальном времени по статусу доставки.
           0 (по умолчанию) – обычное сообщение.
           1 – ping-sms. Будет отправлено Ping-SMS на каждый номер телефона в списке. Параметр mes не используется.
         */
        public int Ping { get; set; } = 0;


        /*
         * Признак MMS-сообщения, с помощью которого можно передавать текст (txt), изображения различных форматов (jpg, gif, png), музыку (wav, amr, mp3, mid) и видео (mp4, 3gp). Файлы передаются в теле http-запроса.
            0 (по умолчанию) – обычное сообщение.
            1 – MMS-сообщение. Будет отправлено MMS на каждый номер телефона в списке.
        */
        public int Mms { get; set; } = 0;


        /*
         Признак e-mail сообщения. Файлы, прикрепляемые к сообщению, передаются методом POST в теле http-запроса.
           0 (по умолчанию) – обычное сообщение.
           1 – e-mail сообщение.
         */
        public int Mail { get; set; } = 0;


        /*
         Признак soc-сообщения, отправляемого пользователям социальных сетей "Одноклассники", "ВКонтакте" или пользователям "Mail.Ru Агент".
           0 (по умолчанию) – обычное сообщение.
           1 – soc-сообщение.
         */
        public int Soc { get; set; } = 0;


        /*
         Признак viber-сообщения, отправляемого пользователям мессенджера Viber.
         */
        public int Viber { get; set; } = 0;


        /*
         Полный http-адрес файла для загрузки и передачи в сообщении.
         */
        public string Fileurl { get; set; }


        /*
         Признак голосового сообщения. При формировании голосового сообщения можно передавать как текст, так и прикреплять файлы. Файлы, добавляемые к сообщению, должны передаваться методом POST в теле http-запроса.
            0 (по умолчанию) – обычное сообщение.
            1 – голосовое сообщение.
         */
        public int Call { get; set; } = 0;


        /*
         Голос, используемый для озвучивания текста (только для голосовых сообщений).
            m (по умолчанию) – мужской голос.
            m2 – мужской голос 2.
            m3 – мужской голос 3.
            m4 – мужской голос 4.
            w – женский голос.
            w2 – женский голос 2.
            w3 – женский голос 3.
            w4 – женский голос 4.
         */
        public string Voice { get; set; } = "w";


        /*
         Разделенный запятой список параметров для голосового сообщения в формате "param=w,i,n".
            Здесь:
            w – время ожидания поднятия трубки абонентом после начала звонка в секундах. Если в течение указанного времени абонент не поднимет трубку, то звонок уйдет на повтор с ошибкой "абонент занят". Рабочий диапазон значений параметра от 10 до 35, но можно указывать интервал от 0 до 99 (в случае, если значение меньше 10, то оно будет приведено к 10, аналогично для верхней границы).
            i – интервал повтора, то есть промежуток времени, по истечении которого произойдет повторный звонок (в секундах). Рабочий диапазон параметра от 10 до 3600 (в случае, если значение меньше 10, то оно будет приведено к 10).
            n – общее количество попыток дозвона. Рабочий диапазон значений от 1 до 9 (0 будет приведен к 1).
            При указании значения любого параметра, отличного от возможных, будут использованы значения всех параметров по умолчанию (n = 8, w = 25, i от 3 до 14 секунд по нарастающей).
         */
        public string Param { get; set; }


        /*
         Тема MMS или e-mail сообщения. При отправке e-mail указание темы, текста и адреса отправителя обязательно. Для MMS обязательным является указание темы или текста. Если не указать тему MMS, то в ее качестве будет использовано имя отправителя, переданное в запросе или используемое по умолчанию.
         */
        public string Subj { get; set; }


        /*
         * Кодировка переданного сообщения, если используется отличная от кодировки по умолчанию windows-1251. Варианты: utf-8 и koi8-r.
         */
        public string Charset { get; set; } = "windows-1251";


        /*
         Признак необходимости получения стоимости рассылки.
            0 (по умолчанию) – обычная отправка.
            1 – получить стоимость рассылки без реальной отправки.
            2 – обычная отправка, но добавить в ответ стоимость выполненной рассылки.
            3 – обычная отправка, но добавить в ответ стоимость и новый баланс Клиента.
         */
        public int Cost { get; set; } = 0;


        /*
         Формат ответа сервера об успешной отправке.
            0 – (по умолчанию) в виде строки (OK - 1 SMS, ID - 1234).
            1 – вернуть ответ в виде чисел: ID и количество SMS через запятую (1234,1), при cost = 2 еще стоимость через запятую (1234,1,1.40), при cost = 3 еще новый баланс Клиента (1234,1,1.40,100.50), при cost = 1 стоимость и количество SMS через запятую (1.40,1).
            2 – ответ в xml формате.
            3 – ответ в json формате.
         */
        public int Fmt { get; set; } = 0;


        /*
         Список номеров телефонов и соответствующих им сообщений, разделенных двоеточием или точкой с запятой и представленный в виде:
            phones1:mes1
            phones2:mes2
            ...
            Строки разделяются через символ новой строки (%0A). В качестве phones можно указать несколько номеров телефонов через запятую. Если в сообщении mes требуется передать символ новой строки, то укажите его через \n. В случае невозможности корректировки текста мультистрокового сообщения возможно включение специального режима для передачи такого типа сообщений. Для этого необходимо дополнительно передавать в запросе параметр nl, равный 1.
            В случае необходимости передачи разных имен отправителей (и, возможно, различных часовых поясов абонентов) для разных сообщений можно использовать следующий формат передачи:
            sender1,tz1|phones1:mes1
            sender2,tz2|phones2:mes2
            ...
            В данном случае параметр tz является необязательным.
            Параметр list позволяет выполнять множественную рассылку с разными сообщениями на несколько телефонов одним http-запросом. Сообщениям в запросе присваивается единый идентификатор. Весь параметр должен быть закодирован с помощью функции urlencode.
         */
        public string List { get; set; }


        /*
         Срок "жизни" SMS-сообщения. Определяет время, в течение которого оператор будет пытаться доставить сообщение абоненту. Диапазон от 1 до 24 часов.
         */
        public int Valid { get; set; }


        /*
         Максимальное количество SMS, на которые может разбиться длинное сообщение. Слишком длинные сообщения будут обрезаться так, чтобы не переполнить количество SMS, требуемых для их передачи. Этим параметром вы можете ограничить максимальную стоимость сообщений, так как за каждое SMS снимается отдельная плата.
         */
        public int Maxsms { get; set; }


        /*
         Значение буквенно-цифрового кода, введенного с "captcha" при использовании антиспам проверки. Данный параметр должен использоваться совместно с параметром userip.
         */
        public string Imgcode { get; set; }


        /*
         Значение IP-адреса, для которого будет действовать лимит на максимальное количество сообщений с одного IP-адреса в сутки, установленный в настройках личного кабинета в пункте "Лимиты и ограничения".
         */
        public string Userip { get; set; }


        /*
         	Признак необходимости добавления в ответ сервера списка ошибочных номеров.
            0 (по умолчанию) – не добавлять список (обычный ответ сервера).
            1 – в ответ добавляется список ошибочных номеров телефонов с соответствующими статусами.
         */
        public int Err { get; set; }


        /*
         Признак необходимости добавления в ответ сервера информации по каждому номеру.
            0 (по умолчанию) – не добавлять список (обычный ответ сервера).
            1 – в ответ добавляется список всех номеров телефонов с соответствующими статусами, значениями mcc и mnc, стоимостью, и, в случае ошибочных номеров, кодами ошибок.
         */
        public int Op { get; set; }


        /*
         Осуществляет привязку Клиента в качестве реферала к определенному ID партнера для текущего запроса.
            При передаче данного параметра в виде "pp=<ID партнера>" Клиент с логином login временно становится рефералом партнера с ID <ID партнера>. Отчисления по партнерской программе будут сделаны именно для текущего запроса, постоянной привязки не происходит. Данный параметр позволяет временно устанавливать Клиента в качестве реферала из своих сервисов и программ, где нет возможности зарегистрировать Клиента по реферальной ссылке.
         */
        public int Pp { get; set; }


        /*
         Флаг изменения первоначальных настроек
         */
        public bool IsCustomEdited { get; set; } = false;

        [JsonIgnore]
        public string TitleBtn 
        { 
            get => IsCustomEdited ? "Пользовательские" : "По умолчанию";
        }
    }


    public class WrapList
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }
}
