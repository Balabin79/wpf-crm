using Dental.Models;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.Services.Smsc.Settings
{

    /// <summary>
    /// Самый базовый (верхний) класс в иерархии настроек
    /// Его задача установить логин и пароль для доступа к сервису
    /// </summary>
    public abstract class RequiredSettings
    {
       public readonly ApplicationContext db;

        public RequiredSettings()
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
