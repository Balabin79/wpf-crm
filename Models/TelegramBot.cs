using B6CRM.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("TelegramBots")]
    public class TelegramBot : AbstractBaseModel, IDataErrorInfo
    {

        [Display(Name = "Токен")]
        public string Token
        {
            get { return GetProperty(() => Token); }
            set { SetProperty(() => Token, value?.Trim()); }
        }

        public string Description
        {
            get { return GetProperty(() => Description); }
            set { SetProperty(() => Description, value?.Trim()); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is TelegramBot clone)
            {
                if (ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(Token, clone.Token) &&
                    StringParamsIsEquel(Guid, clone.Guid) &&
                    StringParamsIsEquel(Description, clone.Description)) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }
    }
}
