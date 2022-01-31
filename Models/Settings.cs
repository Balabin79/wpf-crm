using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Settings : AbstractBaseModel, IDataErrorInfo
    {
        [Display(Name = "Название")]
        public string LoginSmsCenter { get; set; }
        public string PasswordSmsCenter { get; set; }
        public string StartPage { get; set; }
        public int? StartWithLastPage { get; set; } = 0;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is Settings clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.LoginSmsCenter, clone.LoginSmsCenter) && 
                    StringParamsIsEquel(this.PasswordSmsCenter, clone.PasswordSmsCenter) && 
                    StringParamsIsEquel(this.StartPage, clone.StartPage) && 
                    this?.StartWithLastPage == clone.StartWithLastPage &&
                    StringParamsIsEquel(this.Guid, clone.Guid)) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }

        [NotMapped]
        public List<string> FieldsChanges { get; set; } = new List<string>();

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(LoginSmsCenter));
            OnPropertyChanged(nameof(PasswordSmsCenter));
            OnPropertyChanged(nameof(StartPage));
        }
    }
}
