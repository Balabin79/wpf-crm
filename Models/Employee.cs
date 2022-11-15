using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using System.Windows.Media;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Dental.Models
{
    [Table("Employees")]
    public class Employee : AbstractBaseModel, IDataErrorInfo
    {
        [NotMapped]
        public ImageSource Image 
        {
            get { return GetProperty(() => Image); }
            set { SetProperty(() => Image, value); }
        }

        [NotMapped]
        public bool IsVisible
        {
            get { return GetProperty(() => IsVisible); }
            set { SetProperty(() => IsVisible, value); }
        }

        [NotMapped]
        public string Fio
        {
            get => ToString();
        }

        [NotMapped]
        public string Photo
        {
            get { return GetProperty(() => Photo); }
            set { SetProperty(() => Photo, value); }
        }


        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName {
            get { return GetProperty(() => FirstName); }
            set { SetProperty(() => FirstName, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }
        }

        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string LastName
        {
            get { return GetProperty(() => LastName); }
            set { SetProperty(() => LastName, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }
        }

        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName
        {
            get { return GetProperty(() => MiddleName); }
            set { SetProperty(() => MiddleName, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }
        }


        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        public string Note { get; set; }

        [Phone(ErrorMessage = @"В поле ""Телефон"" введено некорректное значение")]
        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value); }
        }

        public string Post
        {
            get { return GetProperty(() => Post); }
            set { SetProperty(() => Post, (value ?? "").Trim()); }
        }

        public int? IsInSheduler { get; set; } = 1;
        public bool? IsInArchive { get; set; } = false;

        public int? IsAdmin { get; set; } = 0;
        public int? IsDoctor { get; set; } = 1;
        public int? IsReception { get; set; } = 0;

        public string Password { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
