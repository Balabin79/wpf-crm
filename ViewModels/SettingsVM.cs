using B6CRM.Models;
using B6CRM.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace B6CRM.ViewModels
{
    public class SettingsVM : ViewModelBase, IDataErrorInfo
    {
        public int? Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string OrgName
        {
            get { return GetProperty(() => OrgName); }
            set { SetProperty(() => OrgName, value?.Trim()); }
        }

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string OrgShortName
        {
            get { return GetProperty(() => OrgShortName); }
            set { SetProperty(() => OrgShortName, value?.Trim()); }
        }

        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string OrgAddress
        {
            get { return GetProperty(() => OrgAddress); }
            set { SetProperty(() => OrgAddress, value?.Trim()); }
        }

        [Phone]
        [Display(Name = "Телефон")]
        public string OrgPhone
        {
            get { return GetProperty(() => OrgPhone); }
            set { SetProperty(() => OrgPhone, value?.Trim()); }
        }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string OrgEmail
        {
            get { return GetProperty(() => OrgEmail); }
            set { SetProperty(() => OrgEmail, value?.Trim()); }
        }

        [Display(Name = "Сайт")]
        public string OrgSite
        {
            get { return GetProperty(() => OrgSite); }
            set { SetProperty(() => OrgSite, value?.Trim()); }
        }

        public int? RolesEnabled
        {
            get { return GetProperty(() => RolesEnabled); }
            set { SetProperty(() => RolesEnabled, value); }
        }

        public int? IsPasswordRequired
        {
            get { return GetProperty(() => IsPasswordRequired); }
            set { SetProperty(() => IsPasswordRequired, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public void Copy(Setting model)
        {
            Id = model.Id;
            OrgName = model.OrgName;
            OrgShortName = model.OrgShortName;
            OrgAddress = model.OrgAddress;
            OrgPhone = model.OrgPhone;
            OrgEmail = model.OrgEmail;
            OrgSite = model.OrgSite;
            RolesEnabled = model.RolesEnabled;
            IsPasswordRequired = model.IsPasswordRequired;
        }
    }
}
