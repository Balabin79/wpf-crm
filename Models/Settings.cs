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
                if (object.ReferenceEquals(this, clone)) return true;

                StringParamsIsEquel(this.LoginSmsCenter, clone.LoginSmsCenter, "Логин");
                StringParamsIsEquel(this.PasswordSmsCenter, clone.PasswordSmsCenter, "Пароль");
                StringParamsIsEquel(this.StartPage, clone.StartPage, "Начинать со страницы");
                if (this?.StartWithLastPage == clone.StartWithLastPage) FieldsChanges.Add("Запуск программы");
            }
            return FieldsChanges.Count == 0;
        }

        private void StringParamsIsEquel(string param1, string param2, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            FieldsChanges.Add(fieldName);
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
