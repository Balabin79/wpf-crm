using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Employees")]
    public class Employee : AbstractBaseModel, IDataErrorInfo
    {
        [NotMapped]
        public ImageSource Image 
        {
            get { return GetProperty(() => Image); }
            set { SetProperty(() => Image, value); }
        }

        [NotMapped]
        public bool IsVisible
        {
            get { return GetProperty(() => IsVisible); }
            set { SetProperty(() => IsVisible, value); }
        }

        [NotMapped]
        public string Fio
        {
            get => ToString();
        }

        // Общая информация
        [Display(Name = "Фото")]
        public string Photo 
        {
            get { return GetProperty(() => Photo); }
            set { SetProperty(() => Photo, value); }
        }

        public string FirstName {
            get { return GetProperty(() => FirstName); }
            set { SetProperty(() => FirstName, value?.Trim()); }
        }

        public string LastName
        {
            get { return GetProperty(() => LastName); }
            set { SetProperty(() => LastName, value?.Trim()); }
        }

        public string MiddleName
        {
            get { return GetProperty(() => MiddleName); }
            set { SetProperty(() => MiddleName, value?.Trim()); }
        }

        public string BirthDate { get; set; }

        public string Email
        {
            get { return GetProperty(() => Email); }
            set { SetProperty(() => Email, value?.Trim()); }
        }

        public string Note { get; set; }

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
            set { SetProperty(() => Address, value?.Trim()); }
        }

        public string Post { get; set; }
        public string Sex { get; set; }
        public int? IsInSheduler { get; set; } = 1;
        public bool? IsInArchive { get; set; } = false;
        public int? UseIndividualPrice { get; set; }

        public int? IsAdmin { get; set; } = 0;
        public int? IsDoctor { get; set; } = 1;
        public int? IsReception { get; set; } = 0;

        public string Password { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
