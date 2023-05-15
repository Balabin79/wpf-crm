using B6CRM.Infrastructures.Extensions.Notifications;
using B6CRM.Models;
using B6CRM.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B6CRM.ViewModels.SmsSenders
{
    public class ServicePassViewModel : ViewModelBase
    {
        private readonly ApplicationContext db;
        private readonly string ServiceName;

        public ServicePassViewModel(string serviceName)
        {
            db = new ApplicationContext();
            ServiceName = serviceName;
            ServicePass = db.ServicesPasses.FirstOrDefault(f => f.Name == ServiceName) ?? new ServicePass() { Name = ServiceName };
            ServicePass.PassDecr = PassDecrypt();
        }

        public string PassDecrypt()
        {
            try
            {
                if (!string.IsNullOrEmpty(ServicePass.Pass))
                    return  Encoding.Unicode.GetString(Convert.FromBase64String(ServicePass.Pass));
                return "";
            }
            catch
            {
                return "";
            }
        }

        public string PassEncrypt()
        {
            try
            {
                if (!string.IsNullOrEmpty(ServicePass.PassDecr))
                    return Convert.ToBase64String(Encoding.Unicode.GetBytes(ServicePass.PassDecr));
                return "";
            }
            catch
            {
                return "";
            }
        }

        [Command]
        public void SavePass()
        {
            try
            {
                if (ServicePass.Id == 0 && (!string.IsNullOrEmpty(ServicePass.Pass) || !string.IsNullOrEmpty(ServicePass.Login)))
                {
                    db.ServicesPasses.Add(ServicePass);
                }

                if (!string.IsNullOrEmpty(ServicePass.PassDecr)) ServicePass.Pass = PassEncrypt();

                if (db.SaveChanges() > 0)
                {
                    new Notification() { Content = "Настройки \"" + ServiceName + "\" сохранены в базу данных!" }.run();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "Ошибка при попытке сохранить настройки \"" + ServiceName + "\" в базу данных!", true);
            }
        }

        public ServicePass ServicePass
        {
            get { return GetProperty(() => ServicePass); }
            set { SetProperty(() => ServicePass, value); }
        }
    }
}
