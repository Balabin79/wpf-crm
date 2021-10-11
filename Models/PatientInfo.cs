using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System;

namespace Dental.Models
{
    [Table("PatientInfo")]
    class PatientInfo : AbstractBaseModel, IDataErrorInfo, IEquatable<PatientInfo>
    {
        [Display(Name = "Номер медицинской карты")]
        public string PatientCardNumber { get; set; } //номер карты

        [Display(Name = "Дата создания медицинской карты")]
        public string PatientCardCreatedAt { get; set; } //дата заполнения карты

        [Display(Name = "Имя")]
        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Фамилия"" не более 255 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName { get; set; }

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
        public int? AdvertisingId{ get; set; }
        public Advertising Advertising { get; set; }

        [Display(Name = "Категория клиентов")]
        public int? ClientCategoryId { get; set; }
        public ClientsGroup ClientCategory { get; set; }

        [Display(Name = "Группа скидки")]
        public int? DiscountGroupId { get; set; }
        public DiscountGroups DiscountGroup { get; set; }

        [Display(Name = "Получает рассылки")]
        public bool? IsSubscribe { get; set; } = true;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }





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

            if (string.Compare(this.FirstName, other.LastName, StringComparison.CurrentCulture) == 0 /*&& this.speed.Equals(other.speed)*/)
                return true;
            else
                return false;
        }

    }
}
