using Dental.Infrastructures.Extensions.Notifications;
using Dental.Models;
using Dental.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dental.ViewModels
{
    public class WorkTimeVM : ViewModelBase
    {
        private readonly ApplicationContext db;
        public delegate void SetWorkTime();
        public event SetWorkTime SetWokTimeEvent;
        public readonly static string workTimeDefault = "09:00:00-18:00:00";

        public WorkTimeVM(ApplicationContext ctx) 
        { 
            db = ctx;
            var workTime = db.Branches.FirstOrDefault()?.WorkTime ?? workTimeDefault;
            try
            {
                WorkTimeFrom = workTime.Substring(0, workTime.IndexOf('-'));
                WorkTimeTo = workTime.Substring(workTime.IndexOf('-') + 1);
            }
            catch(Exception e)
            {
                WorkTimeFrom = "09:00:00";
                WorkTimeTo = "18:00:00";
                Log.ErrorHandler(e);
            }
        }
        

        public string WorkTimeFrom
        {
            get { return GetProperty(() => WorkTimeFrom); }
            set { SetProperty(() => WorkTimeFrom, value); }
        }

        public string WorkTimeTo
        {
            get { return GetProperty(() => WorkTimeTo); }
            set { SetProperty(() => WorkTimeTo, value); }
        }

        [Command]
        public void Save(object p)
        {
            try
            {
                var branch = db.Branches.FirstOrDefault() ?? new Branch();

                if (string.IsNullOrEmpty(WorkTimeFrom)) WorkTimeFrom = "09:00:00";
                if (string.IsNullOrEmpty(WorkTimeTo)) WorkTimeTo = "18:00:00";

                branch.WorkTime = WorkTimeFrom + "-" + WorkTimeTo;

                if (branch?.Id == 0) db.Branches.Add(branch);

                if (db.SaveChanges() > 0) 
                {
                    new Notification() { Content = "Настройки рабочего времени сохранены!" }.run();
                    SetWokTimeEvent?.Invoke();
                }
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e, "При попытке сохранении настроек рабочего времени в базу данных произошла ошибка!", true);
            }
            finally { if (p is Window win) win?.Close(); }
        }
    }
}
