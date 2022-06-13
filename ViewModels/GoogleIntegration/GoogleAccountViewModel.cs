using Dental.Models;
using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.ViewModels.GoogleIntegration
{
    public class GoogleAccountViewModel : BindableBase, IDataErrorInfo, ISettings
    {
        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        [Required(ErrorMessage = @"Поле ""Email"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Email"" не более 255 символов")]
        public string Key
        {
            get { return GetProperty(() => Key); }
            set { SetProperty(() => Key, value); }
        }

        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Пароль"" не более 255 символов")]
        [Required(ErrorMessage = @"Поле ""Пароль"" обязательно для заполнения")]
        public string Value
        {
            get { return GetProperty(() => Value); }
            set { SetProperty(() => Value, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
