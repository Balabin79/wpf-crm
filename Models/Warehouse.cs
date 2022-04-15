using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Warehouse")]
    public class Warehouse : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(500, ErrorMessage = @"Длина не более 500 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set 
            {
                _Name = value?.Trim();
            } 
        }
        private string _Name;

        [Display(Name = "Ответственное лицо")]
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Employee));
        }

        public override string ToString()
        {
            return Name;
        }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is Warehouse clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.Name, clone.Name) && StringParamsIsEquel(this.Guid, clone.Guid) && StringParamsIsEquel(this.Employee?.Guid, clone.Employee?.Guid)) return true;
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
