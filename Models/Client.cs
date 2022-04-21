using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dental.Models
{
    [Table("ClientInfo")]
    public class Client : AbstractBaseModel, IDataErrorInfo, ICloneable, IEquatable<Client>
    {
        [Display(Name = "Номер карты клиента")]
        public string ClientCardNumber { get; set; } //номер карты

        [Display(Name = "Дата создания клиентской карты")]
        public string ClientCardCreatedAt { get; set; } //дата заполнения карты

        [Display(Name = "Имя")]
        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName        
        {
            get => _FirstName;
            set
            {
                _FirstName = value;
                if (_FirstName?.Length > 0) _FirstName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value).Trim();
            } 
        }
        private string _FirstName;

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Фамилия"" не более 255 символов")]
        public string LastName 
        {
            get => _LastName;
            set
            {
                _LastName = value;
                if (_LastName?.Length > 0) _LastName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value).Trim();
            }

        }
        private string _LastName;

        [Display(Name = "Отчество")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName
        {
            get => _MiddleName;
            set
            {
                _MiddleName = value;
                if (_MiddleName?.Length > 0) _MiddleName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value).Trim();
            }

        }
        private string _MiddleName;

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; }

        [NotMapped]
        public string FullName
        {
            get {
                if (FirstName == null && LastName == null && MiddleName == null) return "";
                return (string.IsNullOrEmpty(MiddleName)) ? LastName + " " + FirstName : LastName + " " + FirstName + " " + MiddleName; 
            }
        }

        [NotMapped]
        public string Title
        {
            get => String.IsNullOrEmpty(FullName) ? "Новый клиент" : ("Карта клиента №" + Id + " ("+ FullName +")");
        }

        [Display(Name = "Пол")]
        public string Sex { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone
        {
            get => _Phone;
            set => _Phone = value?.Trim();
        }
        private string _Phone;

        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email")]
        public string Email
        {
            get => _Email;
            set => _Email = value?.Trim();
        }
        private string _Email;

        [Display(Name = "Адрес проживания")]
        public string Address
        {
            get => _Address;
            set => _Address = value?.Trim();
        }
        private string _Address;

        [Display(Name = "Примечание")]
        public string Note
        {
            get => _Note;
            set => _Note = value?.Trim();
        }
        private string _Note;

        [Display(Name = "Канал привлечения")]
        public int? AdvertisingId { get; set; }
        public Advertising Advertising { get; set; }

        [Display(Name = "Перемещена в архив")]
        public bool? IsInArchive { get; set; } = false;

        [Display(Name = "Серия паспорта")]
        public string PassportSeries { get; set; }

        [Display(Name = "Номер паспорта")]
        public string PassportNo { get; set; }

        [Display(Name = "Дата выдачи паспорта")]
        public string PassportIssuanceDate { get; set; }

        [Display(Name = "Кем выдан")]
        public string WhomIssued { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public bool Equals(Client other)
        {
            if (FieldsChanges != null) FieldsChanges = CreateFieldsChanges();
            if (other is Client clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;

                StringParamsIsEquel(this.FirstName, other.FirstName, "Административная", "Имя");
                StringParamsIsEquel(this.LastName, other.LastName, "Административная", "Фамилия");
                StringParamsIsEquel(this.MiddleName, other.MiddleName, "Административная", "Отчество");
                StringParamsIsEquel(this.BirthDate, other.BirthDate, "Административная", "Дата рождения");
                StringParamsIsEquel(this.Sex, other.Sex, "Административная", "Пол");
                StringParamsIsEquel(this.Phone, other.Phone, "Административная", "Телефон");
                StringParamsIsEquel(this.Email, other.Email, "Административная", "Email");
                StringParamsIsEquel(this.Address, other.Address, "Административная", "Адрес проживания");
                StringParamsIsEquel(this.Note, other.Note, "Административная", "Примечание");
                StringParamsIsEquel(this.Advertising?.Guid, other.Advertising?.Guid, "Административная", "Рекламные источники");
                StringParamsIsEquel(this.PassportIssuanceDate, other.PassportIssuanceDate, "Административная", "Дата выдачи паспорта");
                StringParamsIsEquel(this.WhomIssued, other.WhomIssued, "Административная", "Кем выдан");
                StringParamsIsEquel(this.PassportSeries, other.PassportSeries, "Административная", "Серия паспорта");
                StringParamsIsEquel(this.PassportNo, other.PassportNo, "Административная", "Номер паспорта");
                
                if (this.IsInArchive != other.IsInArchive) FieldsChanges["Административная"].Add("Перемещена в архив");
            }
            return FieldsChanges["Административная"].Count == 0 && FieldsChanges["Планы услуг"].Count == 0;
        }

        private void StringParamsIsEquel(string param1, string param2, string section, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            FieldsChanges[section].Add(fieldName);
        }

        [NotMapped]
        public Dictionary<string, List<string>> FieldsChanges { get; set; } = CreateFieldsChanges();

        public static Dictionary<string, List<string>> CreateFieldsChanges()
        {
            return new Dictionary<string, List<string>>() {
                { "Административная", new List<string>() },
                { "Планы услуг", new List<string>() },
            };
        }

        public override string ToString() => FullName;

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(MiddleName));
        }
    }
}
