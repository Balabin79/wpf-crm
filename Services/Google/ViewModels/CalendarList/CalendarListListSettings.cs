using Dental.Models;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.CalendarList
{
    public class CalendarListListSettings : BaseViewModel
    { 
        public int MaxResults
        {
            get { return GetProperty(() => MaxResults); }
            set { SetProperty(() => MaxResults, value); }
        }

        public string MinAccessRole
        {
            get { return GetProperty(() => MinAccessRole); }
            set { SetProperty(() => MinAccessRole, value); }
        }

        public string PageToken
        {
            get { return GetProperty(() => PageToken); }
            set { SetProperty(() => PageToken, value); }
        }

        public bool ShowDeleted
        {
            get { return GetProperty(() => ShowDeleted); }
            set { SetProperty(() => ShowDeleted, value); }
        }

        public bool ShowHidden
        {
            get { return GetProperty(() => ShowHidden); }
            set { SetProperty(() => ShowHidden, value); }
        }

        public string SyncToken
        {
            get { return GetProperty(() => SyncToken); }
            set { SetProperty(() => SyncToken, value); }
        }
    }
}
