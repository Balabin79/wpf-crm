using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("AppointmentsStatuses")]
    public class AppointmentStatus : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Название"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Caption 
        {
            get { return GetProperty(() => Caption); }
            set { SetProperty(() => Caption, value?.Trim()); }
        }

        public string BrushColor
        {
            get { return GetProperty(() => BrushColor); }
            set { SetProperty(() => BrushColor, value?.Trim()); }
        }

        [NotMapped]
        public SolidColorBrush Brush 
        {
            get { return GetProperty(() => Brush); }
            set { SetProperty(() => Brush, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is AppointmentStatus clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.Caption, clone.Caption) && 
                    StringParamsIsEquel(this.Guid, clone.Guid))  return true;
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
