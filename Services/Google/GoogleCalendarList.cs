using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Threading;

namespace Dental.Services.Google
{
    // Коллекция календарей в списке календарей пользователя
    public class GoogleCalendarList
    {
        private readonly CalendarService service;
        private readonly INotification notification;

        public GoogleCalendarList(CalendarService service, INotification notification) 
        {
            this.service = service;
            this.notification = notification;
        }

        public async Task<CalendarList> ListAsync(int maxResults = 100) 
        {
            var list = service.CalendarList.List();
            list.MaxResults = maxResults;                
            return await list.ExecuteAsync();
        }

        public async Task DeleteAsync(string id) => await service.Calendars.Delete(id).ExecuteAsync();


        public async Task<Calendar> GetAsync(string id) => await service.Calendars.Get(id).ExecuteAsync();
            
        public void UpdateAsync() 
        { 
        }
        public void WatchAsync() { }

        public async Task Wait()
        {
            await Task.Delay(7000);
            notification.ShowMsgError("Wait");
        }

    }
}
