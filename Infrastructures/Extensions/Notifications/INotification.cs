using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dental.Infrastructures.Extensions.Notifications
{
    interface INotification
    {
        string Caption { get; set; }
        string Content { get; set; }
        ImageSource Icon { get; set; }
        INotificationService NotificationService { get; }
        void run();
    }
}
