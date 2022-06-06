using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.Acl
{
    public class AclPatchSettings : BaseViewModel
    {
        public string RuleId
        {
            get { return GetProperty(() => RuleId); }
            set { SetProperty(() => RuleId, value); }
        }

        public bool SendNotifications
        {
            get { return GetProperty(() => SendNotifications); }
            set { SetProperty(() => SendNotifications, value); }
        }
    }
}
