using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Dental.Models
{
    [Table("PatientInfo")]
    class PatientInfo : AbstractBaseModel, IDataErrorInfo
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
    }
}
