using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Services.Google.ViewModels.Acl
{
    public class AclInsertSettings : BaseViewModel
    {
        public bool SendNotifications
        {
            get { return GetProperty(() => SendNotifications); }
            set { SetProperty(() => SendNotifications, value); }
        }

        //Адрес электронной почты пользователя или группы или имя домена, в зависимости от типа области. Опущен для типа " default".
        public string Value
        {
            get { return GetProperty(() => Value); }
            set { SetProperty(() => Value, value); }
        }

        public string Role
        {
            get { return GetProperty(() => Role); }
            set { SetProperty(() => Role, value); }
        }

        public string Scope
        {
            get { return GetProperty(() => Scope); }
            set { SetProperty(() => Scope, value); }
        }
    }
}
