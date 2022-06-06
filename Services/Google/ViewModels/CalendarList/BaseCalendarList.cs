using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Google.ViewModels.CalendarList
{
    public class BaseCalendarList : BindableBase
    {
        public bool CalendarId
        {
            get { return GetProperty(() => CalendarId); }
            set { SetProperty(() => CalendarId, value); }
        }
    }
}
