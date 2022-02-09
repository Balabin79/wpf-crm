using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("SmsCenter")]
    public class SmsCenter : AbstractBaseModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is SmsCenter clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.Login, clone.Login) &&
                    StringParamsIsEquel(this.Password, clone.Password)
                ) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }


        public void UpdateFields()
        {
            OnPropertyChanged(nameof(Login));
            OnPropertyChanged(nameof(Password));
        }
    }
}
