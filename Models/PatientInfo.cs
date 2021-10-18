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
    [Table("PatientInfo")]
    class PatientInfo : AbstractBaseModel, IDataErrorInfo, ICloneable, IEquatable<PatientInfo>
    {

        [Display(Name = "Номер медицинской карты")]
        public string PatientCardNumber { get; set; } //номер карты

        [Display(Name = "Дата создания медицинской карты")]
        public string PatientCardCreatedAt { get; set; } //дата заполнения карты

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
        public string _MiddleName;

        [Display(Name = "Дата рождения")]
        public string BirthDate { get; set; }

        [NotMapped]
        public string FullName
        {
            get => (string.IsNullOrEmpty(MiddleName)) ? LastName + " " + FirstName : LastName + " " + FirstName + " " + MiddleName;
        }

        [Display(Name = "Пол")]
        public string Sex { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Адрес проживания")]
        public string Address { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [Display(Name = "Канал привлечения")]
        public string Advertising { get; set; }

        [Display(Name = "Категория клиентов")]
        public string ClientCategory { get; set; }

        [Display(Name = "Группа скидки")]
        public string DiscountGroup { get; set; }

        [Display(Name = "Получает рассылки")]
        public bool? IsSubscribe { get; set; } = true;

        [Display(Name = "Перемещена в архив")]
        public bool? IsInArchive { get; set; } = false;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }



        public object Clone()
        {

            return new PatientInfo
            {
                PatientCardNumber = this.PatientCardNumber,
                PatientCardCreatedAt = this.PatientCardCreatedAt,
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
                DiscountGroup = this.DiscountGroup
            };

            /*Company company = new Company { Name = this.Work.Name };
            return new Person
            {
                Name = this.Name,
                Age = this.Age,
                Work = company
            };*/
        }

        public override bool Equals(object other)
        {
            if (FieldsChanges != null) FieldsChanges = CreateFieldsChanges();
            

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
            bool notIsChanges = true;
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

            if (string.Compare(this.FirstName, other.FirstName, StringComparison.CurrentCulture) != 0 /*&& this.speed.Equals(other.speed)*/)
            {
                notIsChanges = false;
               FieldsChanges["Административная"].Add("Имя");
            }
            if (string.Compare(this.LastName, other.LastName, StringComparison.CurrentCulture) != 0 )
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Фамилия");
            }
            if (string.Compare(this.MiddleName, other.MiddleName, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Отчество");
            }
            if (string.Compare(this.BirthDate, other.BirthDate, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Дата рождения");
            }
            if (string.Compare(this.Sex, other.Sex, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Пол");
            }
            if (string.Compare(this.Phone, other.Phone, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Телефон");
            }
            if (string.Compare(this.Email, other.Email, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Email");
            }
            if (string.Compare(this.Address, other.Address, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Адрес");
            }
            if (string.Compare(this.Note, other.Note, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Дополнительно");
            }
            if (string.Compare(this.Advertising, other.Advertising, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Рекламные источники");
            }
            if (string.Compare(this.ClientCategory, other.ClientCategory, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Категории клиентов");
            }
            if (string.Compare(this.DiscountGroup, other.DiscountGroup, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Группы скидок");
            }
            if (this.IsSubscribe != other.IsSubscribe)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Участие в рассылках");
            }            
            if (this.IsInArchive != other.IsInArchive)
            {
                notIsChanges = false;
                FieldsChanges["Административная"].Add("Перемещена в архив");
            }
            return notIsChanges;
        }

        [NotMapped]
        public Dictionary<string, List<string>> FieldsChanges { get; set; } = CreateFieldsChanges();

        private static Dictionary<string, List<string>> CreateFieldsChanges()
        {
            return new Dictionary<string, List<string>>() {
                { "Административная", new List<string>() },
                { "План лечения и счета", new List<string>() },
                { "Карта зубов", new List<string>() }
            };
        }

    }
}
