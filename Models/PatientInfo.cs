using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Dental.Models
{
    [Table("ClientInfo")]
    public class PatientInfo : AbstractBaseModel, IDataErrorInfo, ICloneable, IEquatable<PatientInfo>
    {
        public PatientInfo()
        {
            TreatmentPlans = new ObservableCollection<TreatmentPlan>();
        }

        public ObservableCollection<TreatmentPlan> TreatmentPlans { get; set; }

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
        public string Advertising
        {
            get => _Advertising;
            set => _Advertising = value?.Trim();
        }
        private string _Advertising;

        [Display(Name = "Категория клиентов")]
        public string ClientCategory { get; set; }

        [Display(Name = "Получает рассылки")]
        public bool? IsSubscribe { get; set; } = true;

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

        public override string ToString()
        {
            return (string.IsNullOrEmpty(MiddleName)) ? LastName + " " + FirstName : LastName + " " + FirstName + " " + MiddleName;
        }

        public object Clone()
        {

            return new PatientInfo
            {
                Id = this.Id,
                ClientCardNumber = this.ClientCardNumber,
                ClientCardCreatedAt = this.ClientCardCreatedAt,
                FirstName = this.FirstName,
                LastName = this.LastName,
                MiddleName = this.MiddleName,
                BirthDate = this.BirthDate,
                Sex = this.Sex,
                Phone = this.Phone,
                Email = this.Email,
                Address = this.Address,
                Note = this.Note,
                IsSubscribe = this.IsSubscribe,
                IsInArchive = this.IsInArchive,
                Advertising = this.Advertising,
                ClientCategory = this.ClientCategory,
                PassportIssuanceDate = this.PassportIssuanceDate,
                PassportNo = this.PassportNo,
                PassportSeries = this.PassportSeries,
                WhomIssued = this.WhomIssued
            };

            /*Company company = new Company { Name = this.Work.Name };
            return new Person
            {
                Name = this.Name,
                Age = this.Age,
                Work = company
            };*/
        }

        public PatientInfo Copy(PatientInfo model)
        {
            model.Id = this.Id;
            model.ClientCardNumber = this.ClientCardNumber;
            model.ClientCardCreatedAt = this.ClientCardCreatedAt;
            model.FirstName = this.FirstName;
            model.LastName = this.LastName;
            model.MiddleName = this.MiddleName;
            model.BirthDate = this.BirthDate;
            model.Sex = this.Sex;
            model.Phone = this.Phone;
            model.Email = this.Email;
            model.Address = this.Address;
            model.Note = this.Note;
            model.IsSubscribe = this.IsSubscribe;
            model.IsInArchive = this.IsInArchive;
            model.Advertising = this.Advertising;
            model.ClientCategory = this.ClientCategory;
            model.PassportIssuanceDate = this.PassportIssuanceDate;
            model.PassportNo = this.PassportNo;
            model.PassportSeries = this.PassportSeries;
            model.WhomIssued = this.WhomIssued;
            return model;
        }

        public override bool Equals(object other)
        {

            //Последовательность проверки должна быть именно такой.
            //Если не проверить на null объект other, то other.GetType() может выбросить //NullReferenceException.            
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            //Если класс находится на вершине иерархии или просто не имеет наследников, то можно просто
            //сделать Vehicle tmp = other as Vehicle; if(tmp==null) return false; 
            //Затем вызвать экземплярный метод, сразу передав ему объект tmp.
            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as PatientInfo);
        }
        public bool Equals(PatientInfo other)
        {
            if (FieldsChanges != null) FieldsChanges = CreateFieldsChanges();
            NotIsChanges = true;
            if (other == null)
                return false;

            //Здесь сравнение по ссылкам необязательно.
            //Если вы уверены, что многие проверки на идентичность будут отсекаться на проверке по ссылке - //можно имплементировать.
            if (object.ReferenceEquals(this, other))
                return true;

            //Если по логике проверки, экземпляры родительского класса и класса потомка могут считаться равными,
            //то проверять на идентичность необязательно и можно переходить сразу к сравниванию полей.
            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.FirstName, other.FirstName, "Административная", "Имя");
            StringParamsIsEquel(this.LastName, other.LastName, "Административная", "Фамилия");
            StringParamsIsEquel(this.MiddleName, other.MiddleName, "Административная", "Отчество");
            StringParamsIsEquel(this.BirthDate, other.BirthDate, "Административная", "Дата рождения");
            StringParamsIsEquel(this.Sex, other.Sex, "Административная", "Пол");
            StringParamsIsEquel(this.Phone, other.Phone, "Административная", "Телефон");
            StringParamsIsEquel(this.Email, other.Email, "Административная", "Email");
            StringParamsIsEquel(this.Address, other.Address, "Административная", "Адрес проживания");
            StringParamsIsEquel(this.Note, other.Note, "Административная", "Примечание");
            StringParamsIsEquel(this.Advertising, other.Advertising, "Административная", "Рекламные источники");
            StringParamsIsEquel(this.ClientCategory, other.ClientCategory, "Административная", "Категории клиентов");
            StringParamsIsEquel(this.PassportIssuanceDate, other.PassportIssuanceDate, "Административная", "Дата выдачи паспорта");
            StringParamsIsEquel(this.WhomIssued, other.WhomIssued, "Административная", "Кем выдан");
            StringParamsIsEquel(this.PassportSeries, other.PassportSeries, "Административная", "Серия паспорта");
            StringParamsIsEquel(this.PassportNo, other.PassportNo, "Административная", "Номер паспорта");

            if (this.IsSubscribe != other.IsSubscribe)
            {
                NotIsChanges = false;
                FieldsChanges["Административная"].Add("Участие в рассылках");
            }            
            if (this.IsInArchive != other.IsInArchive)
            {
                NotIsChanges = false;
                FieldsChanges["Административная"].Add("Перемещена в архив");
            }
            return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2, string section, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
            FieldsChanges[section].Add(fieldName);
        }

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;

        [NotMapped]
        public Dictionary<string, List<string>> FieldsChanges { get; set; } = CreateFieldsChanges();

        public static Dictionary<string, List<string>> CreateFieldsChanges()
        {
            return new Dictionary<string, List<string>>() {
                { "Административная", new List<string>() },
                { "Планы услуг", new List<string>() },
            };
        }

    }
}
