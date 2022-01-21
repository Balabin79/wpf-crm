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

        public object Clone()
        {
            return new Settings
            {
                Id = this.Id,
                LoginSmsCenter = this.LoginSmsCenter,
                PasswordSmsCenter = this.PasswordSmsCenter,
                StartPage = this.StartPage,
                StartWithLastPage = this.StartWithLastPage,
                Guid = this.Guid,
            };
        }

        public Settings Copy(Settings model)
        {
            model.Id = this.Id;
            model.LoginSmsCenter = this.LoginSmsCenter;
            model.PasswordSmsCenter = this.PasswordSmsCenter;
            model.StartPage = this.StartPage;
            model.StartWithLastPage = this.StartWithLastPage;
            model.Guid = this.Guid;
            return model;
        }


        public override bool Equals(object other)
        {           
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as Settings);
        }
        public bool Equals(Settings other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            if (this.StartWithLastPage != other.StartWithLastPage) return false;

            StringParamsIsEquel(this.LoginSmsCenter, other.LoginSmsCenter);
            StringParamsIsEquel(this.PasswordSmsCenter, other.PasswordSmsCenter);
            StringParamsIsEquel(this.StartPage, other.StartPage);
            StringParamsIsEquel(this.Guid, other.Guid);
            return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
        }

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;

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
