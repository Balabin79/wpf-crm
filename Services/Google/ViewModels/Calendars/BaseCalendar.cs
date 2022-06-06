using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.Calendar
{
    public class BaseCalendar : BindableBase
    {
        public bool CalendarId
        {
            get { return GetProperty(() => CalendarId); }
            set { SetProperty(() => CalendarId, value); }
        }
    }
}
