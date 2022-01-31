using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Employes")]
    public class Employee : AbstractBaseModel, IDataErrorInfo
    {
        [NotMapped]
        public ImageSource Image { get; set; }

        [NotMapped]
        public bool IsVisible 
        {
            get => isVisible; 
            set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            } 
        }
        private bool isVisible;

        [NotMapped]
        public string Specialities { get; set; }

        [NotMapped]
        public string Fio { 
            get => (string.IsNullOrEmpty(MiddleName)) ? LastName + " " + FirstName : LastName + " " + FirstName + " " + MiddleName; 
        }
        // Общая информация
        [Display(Name = "Фото")]
        public string Photo { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName {
            get => _FirstName;
            set => _FirstName = value?.Trim();
        }
        private string _FirstName;

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Фамилия"" не более 255 символов")]
        public string LastName
        {
            get => _LastName;
            set => _LastName = value?.Trim();
        }
        private string _LastName;

        [Display(Name = "Отчество")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName
        {
            get => _MiddleName;
            set => _MiddleName = value?.Trim();
        }
        private string _MiddleName;

        [Display(Name = "Размер оклада")]
        public string Amount
        {
            get => _Amount;
            set => _Amount = value?.Trim();
        }
        private string _Amount;

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; }

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? FirstName + " " + LastName : FirstName + " " + MiddleName + " " + LastName;
        }
        // Контактная информация
        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email
        {
            get => _Email;
            set => _Email = value?.Trim();
        }
        private string _Email;

        public string Skype
        {
            get => _Skype;
            set => _Skype = value?.Trim();
        }
        private string _Skype;

        [Display(Name = "Примечание")]
        public string Note
        {
            get => _Note;
            set => _Note = value?.Trim();
        }
        private string _Note;

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = @"В поле ""Телефон"" введено некорректное значение")]
        public string Phone { get; set; }

        [Display(Name = "Доп.телефон")]
        [Phone(ErrorMessage = @"В поле ""Дополнительный телефон"" введено некорректное значение")]
        public string AddPhone { get; set; }

        [Display(Name = "Дата приема")]
        public string HireDate { get; set; } // дата приема на работу

        [Display(Name = "Уволен")]
        public int? IsDismissed { get; set; }

        [Display(Name = "Дата увольнения")]
        public string DismissalDate { get; set; } //= DateTime.Now.ToShortDateString().ToString(); // дата увольнения

        [Display(Name = "ИНН")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = @"Формат ""ИНН""- 12 цифр")]
        public string Inn { get; set; }

        [Display(Name = "Адрес")]
        public string Address
        {
            get => _Address;
            set => _Address = value?.Trim();
        }
        private string _Address;

        [Display(Name = "Категория сотрудника")]
        public EmployeeGroup EmployeeGroup { get; set; }
        public int? EmployeeGroupId { get; set; }

        [Display(Name = "Статус")]
        public Dictionary Status { get; set; }
        public int? StatusId { get; set; }

        [Display(Name = "Пол")]
        public Dictionary Sex { get; set; }
        public int? SexId { get; set; }

        [Display(Name = "Оклад фиксированный")]
        public int? IsFixRate { get; set; }

        public List<EmployesSpecialities> EmployesSpecialities { get; set; } = new List<EmployesSpecialities>();

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
        
        public bool Equals(Employee other)
        {
            if (FieldsChanges != null) FieldsChanges = new List<string>();

            if (other is Employee clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;

                StringParamsIsEquel(this.FirstName, other.FirstName, "Имя");
                StringParamsIsEquel(this.LastName, other.LastName, "Фамилия");
                StringParamsIsEquel(this.MiddleName, other.MiddleName, "Отчество");
                StringParamsIsEquel(this.BirthDate, other.BirthDate, "Дата рождения");
                StringParamsIsEquel(this.Photo, other.Photo, "Фото");
                StringParamsIsEquel(this.Phone, other.Phone, "Домашний телефон");
                StringParamsIsEquel(this.AddPhone, other.AddPhone, "Рабочий телефон");
                StringParamsIsEquel(this.Email, other.Email, "Email");
                StringParamsIsEquel(this.Address, other.Address, "Адрес");
                StringParamsIsEquel(this.Inn, other.Inn, "ИНН");
                StringParamsIsEquel(this.DismissalDate, other.DismissalDate, "Дата увольнения");
                StringParamsIsEquel(this.Status?.Guid, other.Status?.Guid, "Статус");
                StringParamsIsEquel(this.Sex?.Guid, other.Sex?.Guid, "Пол");
                StringParamsIsEquel(this.EmployeeGroup?.Guid, other.EmployeeGroup?.Guid, "Категория сотрудника");
                StringParamsIsEquel(this.Amount, other.Amount, "Размер оклада");
                StringParamsIsEquel(this.Note, other.Note, "Примечание");

                if (this.IsDismissed != other.IsDismissed) FieldsChanges.Add("Уволен");                
                if (this.EmployeeGroupId != other.EmployeeGroupId) FieldsChanges.Add("Категории сотрудников");  
                if (this.IsFixRate != other.IsFixRate) FieldsChanges.Add("Тип оклада");
            }
            return FieldsChanges.Count == 0;
        }

        private void StringParamsIsEquel(string param1, string param2, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            FieldsChanges.Add(fieldName);
        }

        [NotMapped]
        public List<string> FieldsChanges { get; set; } = new List<string>();

        public override string ToString() => Fio;
    }
}
