using Dental.Models;
using Dental.Services.GoogleIntagration.Contacts;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace Dental.Services.GoogleIntagration
{
    public class GoogleIntegrator
    {
        public GoogleIntegrator()
        {
            PingService = new PingService();
            ContactsIntegration = new ContactsIntegration();
        }

        public PingService PingService { get; private set; }
        public ContactsIntegration ContactsIntegration { get; private set; }

        static Timer timer;
        long interval = 30000; //30 секунд
        static object synclock = new object();
        static bool sent = false;



        public void Init()
        {
            timer = new Timer(new TimerCallback(Send), null, 0, interval);
        }

        private void Send(object obj)
        {
            lock (synclock)
            {
                DateTime dd = DateTime.Now;
                if ((dd.Minute % 3) == 0 && sent == false)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ThemedMessageBox.Show(title: "Отправил", text: "Данные отправлены!",
                             messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
                    });


                    //smtp.Send(m);
                     sent = true;
                }
                else if (dd.Hour != 1 && dd.Minute != 30)
                {
                    sent = false;
                }
            }
        }
        public void Dispose()
        { }
    }
}
