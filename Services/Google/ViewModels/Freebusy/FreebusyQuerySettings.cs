using Dental.Models;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.CalendarList
{
    public class FreebusyQuerySettings : BindableBase, IDataErrorInfo
    {
        public DateTime TimeMin
        {
            get { return GetProperty(() => TimeMin); }
            set { SetProperty(() => TimeMin, value); }
        }

        public DateTime TimeMax
        {
            get { return GetProperty(() => TimeMax); }
            set { SetProperty(() => TimeMax, value); }
        }

        public string TimeZone
        {
            get { return GetProperty(() => TimeZone); }
            set { SetProperty(() => TimeZone, value); }
        }

        public int GroupExpansionMax
        {
            get { return GetProperty(() => GroupExpansionMax); }
            set { SetProperty(() => GroupExpansionMax, value); }
        }

        public int CalendarExpansionMax
        {
            get { return GetProperty(() => CalendarExpansionMax); }
            set { SetProperty(() => CalendarExpansionMax, value); }
        }        
        
        public List<string> Items
        {
            get { return GetProperty(() => Items); }
            set { SetProperty(() => Items, value); }
        }

        //items[].id	string	The identifier of a calendar or a group.

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

    }
}
