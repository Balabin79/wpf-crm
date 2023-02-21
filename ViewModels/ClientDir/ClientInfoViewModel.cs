using System;
using Dental.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace Dental.ViewModels.ClientDir
{
    public class ClientInfoViewModel : ViewModelBase, IDataErrorInfo
    {
        public ClientInfoViewModel(Client client)
        {
            Id = client.Id;
            Guid = client.Guid;
            FirstName = client.FirstName;
            LastName = client.LastName;
            MiddleName = client.MiddleName;
            BirthDate = client.BirthDate;
            Gender = client.Gender;
            Phone = client.Phone;
            Address = client.Address;
            Note = client.Note;
            IsInArchive = client.IsInArchive;
            Email = client.Email;
            ClientCategory = client.ClientCategory;
            ClientCategoryId = client.ClientCategoryId;
        }

        public int Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        public string Guid
        {
            get { return GetProperty(() => Guid); }
            set { SetProperty(() => Guid, value); }
        }

        [Required(ErrorMessage = @"Поле ""Имя"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Имя"" не более 255 символов")]
        public string FirstName
        {
            get { return GetProperty(() => FirstName); }
            set { SetProperty(() => FirstName, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }
        }

        [Required(ErrorMessage = @"Поле ""Фамилия"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Фамилия"" не более 255 символов")]
        public string LastName
        {
            get { return GetProperty(() => LastName); }
            set { SetProperty(() => LastName, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }
        }

        [MaxLength(255, ErrorMessage = @"Максимальная длина строки в поле ""Отчество"" не более 255 символов")]
        public string MiddleName
        {
            get { return GetProperty(() => MiddleName); }
            set { SetProperty(() => MiddleName, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value ?? "").Trim()); }
        }

        public string BirthDate
        {
            get { return GetProperty(() => BirthDate); }
            set { SetProperty(() => BirthDate, value); }
        }

        public string Gender { get; set; }

        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value); }
        }

        [Phone]
        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value?.Trim()); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
        }

        public ClientCategory ClientCategory
        {
            get { return GetProperty(() => ClientCategory); }
            set { SetProperty(() => ClientCategory, value); }
        }
        public int? ClientCategoryId { get; set; }
        
        public string Note
        {
            get { return GetProperty(() => Note); }
            set { SetProperty(() => Note, value?.Trim()); }
        }

        public bool? IsInArchive { get; set; } = false;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override int GetHashCode() => Guid.GetHashCode();

        public bool Equals(Client other)
        {
            if (FieldsChanges != null) FieldsChanges = CreateFieldsChanges();

            if (other is Client clone && !object.ReferenceEquals(this, clone))
            {
                StringParamsIsEquel(FirstName, other.FirstName, "Имя");
                StringParamsIsEquel(LastName, other.LastName, "Фамилия");
                StringParamsIsEquel(MiddleName, other.MiddleName, "Отчество");
                StringParamsIsEquel(BirthDate, other.BirthDate, "Дата рождения");
                StringParamsIsEquel(Gender, other.Gender, "Пол");
                StringParamsIsEquel(Phone, other.Phone, "Телефон");
                StringParamsIsEquel(Address, other.Address, "Адрес проживания");
                StringParamsIsEquel(Note, other.Note,  "Примечание");
                StringParamsIsEquel(Email, other.Email, "Email");
                StringParamsIsEquel(ClientCategory?.Guid, other.ClientCategory?.Guid, "Категория клиентов");

                if (this.IsInArchive != other.IsInArchive) FieldsChanges.Add("Перемещена в архив");
            }
            return FieldsChanges.Count == 0 ;
        }

        private void StringParamsIsEquel(string param1, string param2, string fieldName)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            FieldsChanges.Add(fieldName);
        }

        public List<string> FieldsChanges { get; set; } = CreateFieldsChanges();

        public static List<string> CreateFieldsChanges() => new List<string>();

        public ICollection<string> GenderList { get => new List<string> { "Мужчина", "Женщина" }; }

        public override string ToString() => LastName + ' ' + FirstName + " " + MiddleName ?? "";

        public Client Copy(Client client)
        {
            client.FirstName = FirstName;
            client.LastName = LastName;
            client.MiddleName = MiddleName;
            client.BirthDate = BirthDate;
            client.Gender = Gender;
            client.Phone = Phone;
            client.Email = Email;
            client.Address = Address;
            client.Note = Note;
            client.IsInArchive = IsInArchive;
            client.ClientCategory = ClientCategory;
            client.ClientCategoryId = ClientCategoryId;

            return client;
        }
    }
}
