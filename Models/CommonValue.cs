using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Serializable]
    [Table("CommonValues")]
    public class CommonValue : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название поля"" обязательно для заполнения")]
        [Display(Name = "Название")]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value?.Trim()); }
        }

        [Required(ErrorMessage = @"Поле ""Системное имя"" обязательно для заполнения")]
        public string SysName
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value?.Trim()); }
        }

        [Required(ErrorMessage = @"Поле ""Значение"" обязательно для заполнения")]
        public string Value
        {
            get { return GetProperty(() => Value); }
            set { SetProperty(() => Value, value?.Trim()); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


        public override string ToString()
        {
            return Name;
        }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is CommonValue clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.Name, clone.Name) && StringParamsIsEquel(this.Guid, clone.Guid)) return true;
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
