using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.GoogleIntagration.Calendar
{
    class GoogleCalendar : GoogleBase
    {
        
        public GoogleCalendar() : base() => CalendarService = new CalendarService(new BaseClientService.Initializer { HttpClientInitializer = Credential, ApplicationName = APPLICATION_NAME });
        
        private CalendarService CalendarService { get; set; }



        public async Task<CalendarList> ListAsync(int maxResults = 100)
        {
            var list = CalendarService.CalendarList.List();
            list.MaxResults = maxResults;
            return await list.ExecuteAsync();
        }

        public async void GetList()
        {
            var f = ListAsync();
            await f;
            
        }
    }
}
