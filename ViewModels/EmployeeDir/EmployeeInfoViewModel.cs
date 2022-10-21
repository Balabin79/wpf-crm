using System;
using Dental.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.ViewModels.EmployeeDir
{
    public class EmployeeInfoViewModel : ViewModelBase, IDataErrorInfo
    {
        public EmployeeInfoViewModel(Employee employee)
        {
            Id = employee.Id;
            Image = employee.Image;
            Photo = employee.Photo;
            Guid = employee.Guid;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            MiddleName = employee.MiddleName;
            Post = employee.Post;
            IsInSheduler = employee.IsInSheduler;
            BirthDate = employee.BirthDate;
            Sex = employee.Sex;
            Status = employee.Status;
            Phone = employee.Phone;
            Email = employee.Email;
            Address = employee.Address;
            Note = employee.Note;
            IsInArchive = employee.IsInArchive;
            try 
            {
                if (!string.IsNullOrEmpty(employee?.Password))
                    Password = Encoding.UTF8.GetString(Convert.FromBase64String(employee?.Password));
            } catch
            {
                Password = null;
            }
        }

        public int Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }

        [NotMapped]
        public ImageSource Image
        {
            get { return GetProperty(() => Image); }
            set { SetProperty(() => Image, value); }
        }

        [NotMapped]
        public string Photo
        {
            get { return GetProperty(() => Photo); }
            set { SetProperty(() => Photo, value); }
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

        public string Post
        {
            get { return GetProperty(() => Post); }
            set { SetProperty(() => Post, (value ?? "").Trim()); }
        }

        public int? IsInSheduler
        {
            get { return GetProperty(() => IsInSheduler); }
            set { SetProperty(() => IsInSheduler, value); }
        }


        [EmailAddress(ErrorMessage = @"В поле ""Email"" введено некорректное значение")]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value); }
        }


        public string BirthDate
        {
            get { return GetProperty(() => BirthDate); }
            set { SetProperty(() => BirthDate, value); }
        }
        
        public string Sex
        {
            get { return GetProperty(() => Sex); }
            set { SetProperty(() => Sex, value); }
        }

        [Phone(ErrorMessage = @"В поле ""Телефон"" введено некорректное значение")]
        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value); }
        }

        public string Status
        {
            get { return GetProperty(() => Status); }
            set { SetProperty(() => Status, value); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, (value ?? "").Trim()); }
        }

        public string Note
        {
            get { return GetProperty(() => Note); }
            set { SetProperty(() => Note, (value ?? "").Trim()); }
        }

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, (value ?? "").Trim()); }
        }

        public bool? IsInArchive { get; set; } = false;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override int GetHashCode() => Guid.GetHashCode();

        public bool Equals(Employee other)
        {
            if (FieldsChanges != null) FieldsChanges = CreateFieldsChanges();

            if (other is Employee clone && !object.ReferenceEquals(this, clone))
            {
                StringParamsIsEquel(Photo, other.Photo, "Фото");
                StringParamsIsEquel(FirstName, other.FirstName, "Имя");
                StringParamsIsEquel(LastName, other.LastName, "Фамилия");
                StringParamsIsEquel(MiddleName, other.MiddleName, "Отчество");
                StringParamsIsEquel(BirthDate, other.BirthDate, "Дата рождения");
                StringParamsIsEquel(Sex, other.Sex, "Пол");
                StringParamsIsEquel(Phone, other.Phone, "Телефон");
                StringParamsIsEquel(Status, other.Status, "Статус");
                StringParamsIsEquel(Email, other.Email, "Email");
                StringParamsIsEquel(Address, other.Address, "Адрес проживания");
                StringParamsIsEquel(Note, other.Note,  "Примечание");
                StringParamsIsEquel(Post, other.Post,  "Должность");

                if (this.IsInArchive != other.IsInArchive) FieldsChanges.Add("Перемещена в архив");
                if (this.IsInSheduler != other.IsInSheduler) FieldsChanges.Add("В расписании");
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

        public static string[] Statuses
        { 
            get => new string[]{ "Работает", "Уволен", "В отпуске"};  
        }


        public override string ToString() => LastName + ' ' + FirstName + " " + MiddleName ?? "";

        public Employee Copy(Employee employee)
        {
            employee.Image = Image;
            employee.Photo = Photo;
            employee.Guid = Guid;
            employee.FirstName = FirstName;
            employee.LastName = LastName;
            employee.MiddleName = MiddleName;
            employee.Post = Post;
            employee.IsInSheduler = IsInSheduler;
            employee.BirthDate = BirthDate;
            employee.Sex = Sex;
            employee.Phone = Phone;
            employee.Status = Status;
            employee.Address = Address;
            employee.Email = Email;
            employee.Note = Note;
            employee.IsInArchive = IsInArchive;
            try
            {
                string pass = null;

                if (!string.IsNullOrEmpty(employee.Password)) pass = Encoding.UTF8.GetString(Convert.FromBase64String(employee.Password));

                if (pass != Password)
                    employee.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password));
            }
            catch
            {
                employee.Password = null;
            }
            return employee;
        }
    }
}
