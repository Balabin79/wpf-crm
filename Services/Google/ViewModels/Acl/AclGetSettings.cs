using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.Acl
{
    public class AclGetSettings : BaseViewModel
    {
        public string RuleId
        {
            get { return GetProperty(() => RuleId); }
            set { SetProperty(() => RuleId, value); }
        }     
    }
}
