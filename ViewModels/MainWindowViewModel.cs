using System;
using System.Linq;
using Dental.Models;
using DevExpress.Xpf.Core;
using System.Windows;

using Dental.Views.Header;
using DevExpress.Mvvm.DataAnnotations;
using Dental.Services.Google;
using Google.Apis.Calendar.v3.Data;
using System.IO;
using System.Threading.Tasks;
using Dental.Services.GoogleIntagration;
using Service = Dental.Services.GoogleIntagration.Contacts.GoogleContacts;
using Dental.Services.GoogleIntagration.Calendar;

namespace Dental.ViewModels
{
    class MainWindowViewModel : DevExpress.Mvvm.ViewModelBase
    {

        private readonly ApplicationContext db;
        public MainWindowViewModel()
        {
            try
            {
                db = new ApplicationContext();              
            }
            catch
            {
                ThemedMessageBox.Show(title: "Ошибка", text: "Данные в базе данных повреждены! Программа может работать некорректно!",
                        messageBoxButtons: MessageBoxButton.OK, icon: MessageBoxImage.Error);
            }
        }

        [Command]
        public void OpenHelpForm()
        {
            try
            {
                var service = new Service();
                //service.List();
                service.CreateGroup("Тест3");
            //    var t = await r;
                var d = 10;
                //var service = new GoogleCalendar();
               // service.GetList();

                /*var service = new GoogleService().Get();
                var list = new GoogleCalendarList(service, new Notification());
                var task =  list.ListAsync();
                var task2 = list.Wait();*/

                //   HelpWin = new HelpWindow();
                //   HelpWin.ShowDialog();
                /*new Notification().ShowMsgError("1");
                CalendarList = await task;
                await task2;
                new Notification().ShowMsgError("2");*/

            }           
            catch (FileNotFoundException e)
            {
                //Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {

            }
        }

        [Command]
        public void OpenRegForm()
        {
            try
            {
                RegWin = new RegWindow();
                RegWin.ShowDialog();
            }
            catch
            {

            }
        }

        [Command]
        public void OpenAboutForm()
        {
            try
            {
                AboutWin = new AboutWindow();
                AboutWin.ShowDialog();
            }
            catch
            {

            }
        }

        public HelpWindow HelpWin { get; set; }
        public RegWindow RegWin { get; set; }
        public AboutWindow AboutWin { get; set; }

        public CalendarList CalendarList { get; set; }

    }
}
