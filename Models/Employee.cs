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
        public ImageSource Image 
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        private ImageSource image;

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
        public string Fio {
            get => (string.IsNullOrEmpty(MiddleName)) ? LastName + " " + FirstName : LastName + " " + FirstName + " " + MiddleName;
        }

        // Общая информация
        [Display(Name = "Фото")]
        public string Photo 
        {
            get => photo;
            set
            {
                photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }
        private string photo;

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

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; }

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? LastName + " " + FirstName : LastName + " " + FirstName + " " + MiddleName;
        }
        // Контактная информация
        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email
        {
            get => _Email;
            set => _Email = value?.Trim();
        }
        private string _Email;

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


        [Display(Name = "Адрес")]
        public string Address
        {
            get => _Address;
            set => _Address = value?.Trim();
        }
        private string _Address;

        [Display(Name = "Должность")]
        public string Post
        {
            get => _Post;
            set => _Post = value?.Trim();
        }
        private string _Post;


        [Display(Name = "Статус")]
        public Dictionary Status { get; set; }
        public int? StatusId { get; set; }

        [Display(Name = "Пол")]
        public Dictionary Sex { get; set; }
        public int? SexId { get; set; }       

        public int? IsInSheduler { get; set; }

        public int? IsIntegrated { get; set; }
        public int? IsRemoteContactCreated { get; set; }

        [EmailAddress(ErrorMessage = @"В поле ""Email (Google)"" введено некорректное значение")]
        public string GoogleEmail { get; set; }

        public string CalendarName { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

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
                StringParamsIsEquel(this.Phone, other.Phone, "Телефон");
                StringParamsIsEquel(this.Email, other.Email, "Email");
                StringParamsIsEquel(this.Address, other.Address, "Адрес");
                StringParamsIsEquel(this.Post, other.Post, "Должность");
                StringParamsIsEquel(this.Status?.Guid, other.Status?.Guid, "Статус");
                StringParamsIsEquel(this.Sex?.Guid, other.Sex?.Guid, "Пол");
                StringParamsIsEquel(this.Note, other.Note, "Примечание");
        
                if (this.IsInSheduler != other.IsInSheduler) FieldsChanges.Add("В расписании");
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

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(MiddleName));
        }
    }
}
