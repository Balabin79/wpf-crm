using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.Acl
{
    public class AclListSettings : BaseViewModel
    {
        public int MaxResults
        {
            get { return GetProperty(() => MaxResults); }
            set { SetProperty(() => MaxResults, value); }
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

        public string SyncToken
        {
            get { return GetProperty(() => SyncToken); }
            set { SetProperty(() => SyncToken, value); }
        }     
    }
}
