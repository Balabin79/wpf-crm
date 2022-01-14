using Dental.Models;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services.Smsc.SmsSettings
{


    public class Connect
    {
       public readonly ApplicationContext db;

        public Connect()
        {
            db = new ApplicationContext();
            var settings = db.Settings.FirstOrDefault();
            if (settings == null || string.IsNullOrEmpty(settings?.LoginSmsCenter) || string.IsNullOrEmpty(settings?.PasswordSmsCenter))
                throw new SettingsException("Не заполнен логин или пароль для центра уведомлений, продолжение невозможно! Заполните соответствующие поля в разделе \"Настройки\"!");
            
            Login = settings?.LoginSmsCenter;
            Password = settings?.PasswordSmsCenter;
        }

        public string Login { get; set; } 
        public string Password { get; set; } 
    }
}
