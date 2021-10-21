using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Organizations")]
    class Organization : AbstractBaseModel, IDataErrorInfo
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [MaxLength(255)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName { get; set; }

        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "ИНН")]
        public string Inn { get; set; }

        [MaxLength(9, ErrorMessage = "Длина не более 9 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        [Display(Name = "КПП")]
        public string Kpp { get; set; }

        // Общая информация
        [Display(Name = "Лого")]
        public string Logo { get; set; }

        [NotMapped]
        public ImageSource Image { get; set; }

        // Контактная инф-ция
        [Display(Name = "Фактический адрес")]
        public string Address { get; set; }

        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // Банковские реквизиты
        [MaxLength(12, ErrorMessage = "Длина не более 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Bik { get; set; }

        [Display(Name = "Расчетный счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string AccountNumber { get; set; }

        [Display(Name = "Наименование банка")]
        public string BankName { get; set; }

        [Display(Name = "ОГРН")]
        [MaxLength(13, ErrorMessage = "Длина не более 13 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Ogrn { get; set; }

        [Display(Name = "Дата регистрации")]
        public string RegisterDate { get; set; }

        [Display(Name = "Генеральный директор")]
        public string GeneralDirector { get; set; }

        [Display(Name = "Лицензия")]
        public string License { get; set; }

        [Display(Name = "Кем выдана")]
        public string WhoIssuedBy { get; set; }

        [Display(Name = "Дополнительно")]
        public string Additional { get; set; }

        [Display(Name = "ОКВЭД")]
        [MaxLength(6, ErrorMessage = "Длина не более 6 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okved { get; set; }

        [Display(Name = "ОКПО")]
        [MaxLength(10, ErrorMessage = "Длина не более 10 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string Okpo { get; set; }

        [Display(Name = "Корреспондирующий счет")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Разрешено вводить только цифры")]
        public string CorrAccountNumber { get; set; }

        [Display(Name = "Сайт")]
        public string Site { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


        public object Clone()
        {

            return new Organization
            {
                Name = this.Name,
                ShortName = this.ShortName,
                Kpp = this.Kpp,
                Inn = this.Inn,
                Logo = this.Logo,
                Address = this.Address,
                LegalAddress = this.LegalAddress,
                Phone = this.Phone,
                Email = this.Email,
                AccountNumber = this.AccountNumber,
                CorrAccountNumber = this.CorrAccountNumber,
                Bik = this.Bik,
                BankName = this.BankName,
                Ogrn = this.Ogrn,
                RegisterDate = this.RegisterDate,
                GeneralDirector = this.GeneralDirector,
                License = this.License,
                Site = this.Site,
                WhoIssuedBy = this.WhoIssuedBy,
                Additional = this.Additional,
                Okpo = this.Okpo,
                Okved = this.Okved,
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

            return this.Equals(other as Organization);
        }
        public bool Equals(Organization other)
        {
            if (FieldsChanges != null) FieldsChanges = new List<string>();
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

            if (string.Compare(this.Name, other.Name, StringComparison.CurrentCulture) != 0 /*&& this.speed.Equals(other.speed)*/)
            {
                notIsChanges = false;
                FieldsChanges.Add("Наименование");
            }

            if (string.Compare(this.ShortName, other.ShortName, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Сокращенное наименование"); 
            }

            if (string.Compare(this.Kpp, other.Kpp, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("КПП");
            }

            if (string.Compare(this.Inn, other.Inn, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("ИНН");
            }

            if (string.Compare(this.Logo, other.Logo, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Логотип");
            }

            if (string.Compare(this.Phone, other.Phone, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Телефон"); ;
            }

            if (string.Compare(this.Email, other.Email, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Email");
            }

            if (string.Compare(this.Address, other.Address, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Фактический адрес");
            }

            if (string.Compare(this.LegalAddress, other.LegalAddress, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Юридический адрес");
            }

            if (string.Compare(this.Bik, other.Bik, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("БИК");
            }

            if (string.Compare(this.AccountNumber, other.AccountNumber, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Расчетный счет");
            }

            if (string.Compare(this.BankName, other.BankName, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Наименование банка");
            }

            if (string.Compare(this.Ogrn, other.Ogrn, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("ОГРН");
            }

            if (string.Compare(this.Okpo, other.Okpo, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("ОКПО");
            }

            if (string.Compare(this.Okved, other.Okved, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("ОКВЭД");
            }

            if (string.Compare(this.RegisterDate, other.RegisterDate, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Дата регистрации");
            }

            if (string.Compare(this.GeneralDirector, other.GeneralDirector, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Генеральный директор");
            }

            if (string.Compare(this.License, other.License, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Лицензия);");
            }

            if (string.Compare(this.WhoIssuedBy, other.WhoIssuedBy, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Кем выдана");
            }

            if (string.Compare(this.CorrAccountNumber, other.CorrAccountNumber, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Корреспондирующий счет");
            }

            if (string.Compare(this.Additional, other.Additional, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Дополнительно");
            }

            if (string.Compare(this.Site, other.Site, StringComparison.CurrentCulture) != 0)
            {
                notIsChanges = false;
                FieldsChanges.Add("Сайт");
            }

            return notIsChanges;
        }

        [NotMapped]
        public List<string> FieldsChanges { get; set; } = new List<string>();
    }  
}
