using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dental.Services.Google.ViewModels.Calendar;

namespace Dental.Services.Google.ViewModels.Calendars
{
    public class AdvCalendar : BaseCalendar, IDataErrorInfo
    {
        public string Description
        {
            get { return GetProperty(() => Description); }
            set { SetProperty(() => Description, value); }
        }

        public string Location
        {
            get { return GetProperty(() => Location); }
            set { SetProperty(() => Location, value); }
        }

        public List<string> Summary
        {
            get { return GetProperty(() => Summary); }
            set { SetProperty(() => Summary, value); }
        }

        public string TimeZone
        {
            get { return GetProperty(() => TimeZone); }
            set { SetProperty(() => TimeZone, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }    }
}
