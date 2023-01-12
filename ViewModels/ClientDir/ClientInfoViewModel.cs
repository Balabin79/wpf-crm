﻿using System;
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
            Snils = client.Snils;
            Note = client.Note;
            IsInArchive = client.IsInArchive;
            Email = client.Email;
            PassportSeries = client.PassportSeries;
            PassportNo = client.PassportNo;
            PassportIssuanceDate = client.PassportIssuanceDate;
            WhomIssued = client.WhomIssued;
            Image = client.Image;
            Photo = client.Photo;
            Img = client.Img;
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

        public string Note
        {
            get { return GetProperty(() => Note); }
            set { SetProperty(() => Note, value?.Trim()); }
        }

        [MaxLength(11, ErrorMessage = @"Максимальная длина в поле ""СНИЛС"" не более 11 символов")]
        public string Snils
        {
            get { return GetProperty(() => Snils); }
            set { SetProperty(() => Snils, value?.Trim()); }
        }

        public bool? IsInArchive { get; set; } = false;
        public string PassportSeries { get; set; }
        public string PassportNo { get; set; }
        public string PassportIssuanceDate { get; set; }
        public string WhomIssued { get; set; }

        public byte[] Img
        {
            get { return GetProperty(() => Img); }
            set { SetProperty(() => Img, value); }
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
                StringParamsIsEquel(Snils, other.Snils, "СНИЛС");
                StringParamsIsEquel(Note, other.Note,  "Примечание");
                StringParamsIsEquel(PassportIssuanceDate, other.PassportIssuanceDate,  "Дата выдачи паспорта");
                StringParamsIsEquel(WhomIssued, other.WhomIssued, "Кем выдан");
                StringParamsIsEquel(Email, other.Email, "Email");
                StringParamsIsEquel(PassportSeries, other.PassportSeries, "Серия паспорта");
                StringParamsIsEquel(PassportNo, other.PassportNo, "Номер паспорта");

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
            //client.Estimates = ;
            client.FirstName = FirstName;
            client.LastName = LastName;
            client.MiddleName = MiddleName;
            client.BirthDate = BirthDate;
            client.Gender = Gender;
            client.Snils = Snils;
            client.Phone = Phone;
            client.Email = Email;
            client.Address = Address;
            client.Note = Note;
            client.IsInArchive = IsInArchive;
            client.PassportSeries = PassportSeries;
            client.PassportNo = PassportNo;
            client.PassportIssuanceDate = PassportIssuanceDate;
            client.WhomIssued = WhomIssued;
            client.Image = Image;
            client.Photo = Photo;
            client.Img = Img;
            return client;
        }
    }
}
