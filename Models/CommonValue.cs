using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("CommonValues")]
    public class CommonValue : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => name;
            set 
            {
                name = value?.Trim();
                OnPropertyChanged(nameof(Name));
            } 
        }
        private string name;

        public string SysName
        {
            get => sysName;
            set
            {
                sysName = value?.Trim();
                OnPropertyChanged(nameof(SysName));
            }
        }
        private string sysName;

        public string Value
        {
            get => val;
            set
            {
                val = value?.Trim();
                OnPropertyChanged(nameof(Value));
            }
        }
        private string val;

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
