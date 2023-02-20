﻿using Dental.Models;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.ViewModels
{
    public class SettingsVM : ViewModelBase, IDataErrorInfo
    {
        public int? Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        [Required]
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

        public string LoginProviderMsg
        {
            get { return GetProperty(() => LoginProviderMsg); }
            set { SetProperty(() => LoginProviderMsg, value?.Trim()); }
        }
        public string PasswordProviderMsg
        {
            get { return GetProperty(() => PasswordProviderMsg); }
            set { SetProperty(() => PasswordProviderMsg, value?.Trim()); }
        }

        public int? ProviderMsgId
        {
            get { return GetProperty(() => ProviderMsgId); }
            set { SetProperty(() => ProviderMsgId, value); }
        }

        public ProviderMsg ProviderMsg
        {
            get { return GetProperty(() => ProviderMsg); }
            set { SetProperty(() => ProviderMsg, value); }
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
            LoginProviderMsg = model.LoginProviderMsg;
            PasswordProviderMsg = model.PasswordProviderMsg;
            ProviderMsgId = model.ProviderMsgId;
            ProviderMsg = model.ProviderMsg;
        }
    }
}
