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

namespace Dental.Services.Google
{
    class GoogleCalendar
    {
        private readonly CalendarService service;

        public GoogleCalendar(CalendarService service) => this.service = service;

        public async Task Clear(string id = "primary") => await service.Calendars.Clear(id).ExecuteAsync();
         
        public async Task Delete(string id) => await service.Calendars.Delete(id).ExecuteAsync();

        public async Task<Calendar> Get(string id) => await service.Calendars.Get(id).ExecuteAsync();
        

        public void Insert()
        {
          // service.Calendars.Insert(new Calendar{ Summary = calendarName, TimeZone = timeZone, Description = description }).Execute();
        }

        public void Path()
        {

        }

        public void Update()
        {

        }




    }
}
