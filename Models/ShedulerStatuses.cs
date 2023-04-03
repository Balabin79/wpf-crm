using B6CRM.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("ShedulerStatuses")]
    public class ShedulerStatuses : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Заголовок"" обязательно для заполнения")]
        [Display(Name = "Заголовок")]
        public string Caption
        {
            get => caption;
            set => caption = value?.Trim();
        }
        private string caption;

        public string Brush
        {
            get => brush;
            set => brush = value?.Trim();
        }
        private string brush;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public bool Equals(object other)
        {
            if (other is ShedulerStatuses clone)
            {
                if (ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(Caption, clone.Caption) &&
                    StringParamsIsEquel(Brush, clone.Brush) &&
                    StringParamsIsEquel(Guid, clone.Guid)
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
    }
}
