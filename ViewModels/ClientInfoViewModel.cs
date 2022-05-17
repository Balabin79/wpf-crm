using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Dental.Infrastructures.Logs;
using Dental.Models;
using System.Data.Entity;
using DevExpress.Mvvm.Native;
using Dental.Infrastructures.Collection;
using DevExpress.Xpf.Core;
using System.Windows;
using Dental.Infrastructures.Extensions.Notifications;
using Dental.Services;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dental.ViewModels
{
    public class ClientInfoViewModel : ViewModelBase, IDataErrorInfo
    {
        public ClientInfoViewModel(Client client)
        {
            Estimates = new ObservableCollection<Estimate>();
            Id = client.Id;
            Guid = client.Guid;
            FirstName = client.FirstName;
            LastName = client.LastName;
            MiddleName = client.MiddleName;
            BirthDate = client.BirthDate;
            Sex = client.Sex;
            Phone = client.Phone;
            Email = client.Email;
            Address = client.Address;
            Note = client.Note;
            Advertising = client.Advertising;
            AdvertisingId = client.AdvertisingId;
            IsInArchive = client.IsInArchive;
            PassportSeries = client.PassportSeries;
            PassportNo = client.PassportNo;
            PassportIssuanceDate = client.PassportIssuanceDate;
            WhomIssued = client.WhomIssued;
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

        public string BirthDate { get; set; }
        public string Sex { get; set; }

        [Phone]
        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value?.Trim()); }
        }

        [EmailAddress]
        [MaxLength(255)]
        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
        }

        public string Note
        {
            get { return GetProperty(() => Note); }
            set { SetProperty(() => Note, value?.Trim()); }
        }

        public Advertising Advertising { get; set; }
        public int? AdvertisingId { get; set; }

        public bool? IsInArchive { get; set; } = false;
        public string PassportSeries { get; set; }
        public string PassportNo { get; set; }
        public string PassportIssuanceDate { get; set; }
        public string WhomIssued { get; set; }

        public ObservableCollection<Estimate> Estimates
        {
            get { return GetProperty(() => Estimates); }
            set { SetProperty(() => Estimates, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override int GetHashCode() => Guid.GetHashCode();

        public bool Equals(Client other)
        {
            if (FieldsChanges != null) FieldsChanges = CreateFieldsChanges();

            if (other is Client clone && !object.ReferenceEquals(this, clone))
            {
                StringParamsIsEquel(FirstName, other.FirstName, "Административная", "Имя");
                StringParamsIsEquel(LastName, other.LastName, "Административная", "Фамилия");
                StringParamsIsEquel(MiddleName, other.MiddleName, "Административная", "Отчество");
                StringParamsIsEquel(BirthDate, other.BirthDate, "Административная", "Дата рождения");
                StringParamsIsEquel(Sex, other.Sex, "Административная", "Пол");
                StringParamsIsEquel(Phone, other.Phone, "Административная", "Телефон");
                StringParamsIsEquel(Email, other.Email, "Административная", "Email");
                StringParamsIsEquel(Address, other.Address, "Административная", "Адрес проживания");
                StringParamsIsEquel(Advertising?.Guid, other.Advertising?.Guid, "Административная", "Рекламные источники");
                StringParamsIsEquel(Note, other.Note, "Административная", "Примечание");
                StringParamsIsEquel(PassportIssuanceDate, other.PassportIssuanceDate, "Административная", "Дата выдачи паспорта");
                StringParamsIsEquel(WhomIssued, other.WhomIssued, "Административная", "Кем выдан");
                StringParamsIsEquel(PassportSeries, other.PassportSeries, "Административная", "Серия паспорта");
                StringParamsIsEquel(PassportNo, other.PassportNo, "Административная", "Номер паспорта");

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

        public Dictionary<string, List<string>> FieldsChanges { get; set; } = CreateFieldsChanges();

        public static Dictionary<string, List<string>> CreateFieldsChanges()
        {
            return new Dictionary<string, List<string>>() {
                { "Административная", new List<string>() },
                { "Планы услуг", new List<string>() },
            };
        }

        public override string ToString() => LastName + ' ' + FirstName + " " + MiddleName ?? "";

        public Client Copy(Client client)
        {
            client.Estimates = Estimates;
            client.FirstName = FirstName;
            client.LastName = LastName;
            client.MiddleName = MiddleName;
            client.BirthDate = BirthDate;
            client.Sex = Sex;
            client.Phone = Phone;
            client.Email = Email;
            client.Address = Address;
            client.Note = Note;
            client.Advertising = Advertising;
            client.AdvertisingId = AdvertisingId;
            client.IsInArchive = IsInArchive;
            client.PassportSeries = PassportSeries;
            client.PassportNo = PassportNo;
            client.PassportIssuanceDate = PassportIssuanceDate;
            client.WhomIssued = WhomIssued;
            return client;
        }
    }
}
