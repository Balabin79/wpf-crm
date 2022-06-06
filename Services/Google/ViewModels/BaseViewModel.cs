using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Services.Google.ViewModels
{
    public class BaseViewModel : BindableBase, IDataErrorInfo
    {
        public bool CalendarId
        {
            get { return GetProperty(() => CalendarId); }
            set { SetProperty(() => CalendarId, value); }
        }

        public string[] Roles = { "none", "freeBusyReader", "reader", "writer", "owner" }; //для поля select
        public string[] Scopes = { " default", "user", "group", "domain" };

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
