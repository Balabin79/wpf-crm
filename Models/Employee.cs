using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dental.Models.Base;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Employes")]
    class Employee : AbstractBaseModel, IDataErrorInfo
    {
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

        [Display(Name = "Пол")]
        public string Sex { get; set; }

        [Display(Name = "Тип оклада")]
        public string RateType { get; set; }


        public EmployeeGroup EmployeeGroup { get; set; }
        public int EmployeeGroupId { get; set; }

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

        public string Status { get; set; }

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

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


        public object Clone()
        {
            EmployeeGroup EmployeeGroup = new EmployeeGroup { 
                Name = this.EmployeeGroup?.Name,
                IsActive = this.EmployeeGroup?.IsActive,
                IsApplyRule = this.EmployeeGroup?.IsApplyRule,
                MoreOrLess = this.EmployeeGroup?.MoreOrLess,
                PercentOrCost = this.EmployeeGroup?.PercentOrCost,
                Amount = this.EmployeeGroup?.Amount
            };

            return new Employee
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                MiddleName = this.MiddleName,
                Inn = this.Inn,
                Photo = this.Photo,
                Address = this.Address,
                Phone = this.Phone,
                AddPhone = this.AddPhone,
                Email = this.Email,
                BirthDate = this.BirthDate,
                DismissalDate = this.DismissalDate,
                Guid = this.Guid,
                HireDate = this.HireDate,
                Skype = this.Skype,
                Status = this.Status,
                IsDismissed = this.IsDismissed,
                RateType = this.RateType,
                Amount = this.Amount,
                EmployeeGroup = EmployeeGroup,
                EmployeeGroupId = this.EmployeeGroupId,
                Note = this.Note,
                Sex = this.Sex
            };
        }

        public Employee Copy(Employee model)
        {
            model.Id = this.Id;
            model.FirstName = this.FirstName;
            model.LastName = this.LastName;
            model.MiddleName = this.MiddleName;
            model.Inn = this.Inn;
            model.Photo = this.Photo;
            model.Address = this.Address;
            model.Phone = this.Phone;
            model.AddPhone = this.AddPhone;
            model.Email = this.Email;
            model.BirthDate = this.BirthDate;
            model.DismissalDate = this.DismissalDate;
            model.Guid = this.Guid;
            model.HireDate = this.HireDate;
            model.Skype = this.Skype;
            model.Status = this.Status;
            model.RateType = this.RateType;
            model.IsDismissed = this.IsDismissed;
            model.Sex = this.Sex;
            model.Amount = this.Amount;
            model.EmployeeGroup = this.EmployeeGroup;
            model.EmployeeGroupId = this.EmployeeGroupId;
            model.Note = this.Note;
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

            return this.Equals(other as Employee);
        }
        public bool Equals(Employee other)
        {
            if (FieldsChanges != null) FieldsChanges = new List<string>();
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
            StringParamsIsEquel(this.Status, other.Status, "Статус");
            StringParamsIsEquel(this.Sex, other.Sex, "Пол");
            StringParamsIsEquel(this.Amount, other.Amount, "Размер оклада");
            StringParamsIsEquel(this.RateType, other.RateType, "Тип оклада");
            StringParamsIsEquel(this.Note, other.Note, "Примечание");

            if (this.IsDismissed != other.IsDismissed)
            {
                NotIsChanges = false;
                FieldsChanges.Add("Уволен");
            }

            if (this.EmployeeGroupId != other.EmployeeGroupId)
            {
                NotIsChanges = false;
                FieldsChanges.Add("Категории сотрудников");
            }

            return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
            FieldsChanges.Add(fieldName);
        }

        [NotMapped]
        public List<string> FieldsChanges { get; set; } = new List<string>();

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;

    }
}
