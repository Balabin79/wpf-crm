using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Settings : AbstractBaseModel
    {
        [Required(ErrorMessage = @"Поле ""Аккаунт Google"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Аккаунт Google"" не более 255 символов")]
        [EmailAddress(ErrorMessage = @"В поле ""Аккаунт Google"" введено некорректное значение")]
        public string GoogleAccount { get; set; }
        public int? IsUseGoogleIntegration { get; set; } = 0;

        public int? UpdateInterval { get; set; } = 5;

        public string ContactGroupForEmployees { get; set; } = "Контакты сотрудников";
        public string ContactGroupForClients { get; set; } = "Контакты клиентов";
        public string DirectoryNameForEmployeeFiles { get; set; } = "Файлы сотрудников";
        public string DirectoryNameForClientFiles { get; set; } = "Файлы клиентов";

        public string GetFieldsValues()
        {
            return "";
            //GetHashCode GoogleAccount
        }
    }
}
