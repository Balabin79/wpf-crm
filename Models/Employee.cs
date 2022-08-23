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
    [Table("Employes")]
    public class Employee : AbstractBaseModel, IDataErrorInfo
    {
        [NotMapped]
        public ImageSource Image 
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        private ImageSource image;

        [NotMapped]
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        private bool isVisible;

        [NotMapped]
        public string Fio
        {
            get => ToString();
        }

        // Общая информация
        [Display(Name = "Фото")]
        public string Photo 
        {
            get => photo;
            set
            {
                photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }
        private string photo;

        public string FirstName {
            get => _FirstName;
            set => _FirstName = value?.Trim();
        }
        private string _FirstName;

        public string LastName
        {
            get => _LastName;
            set => _LastName = value?.Trim();
        }
        private string _LastName;

        public string MiddleName
        {
            get => _MiddleName;
            set => _MiddleName = value?.Trim();
        }
        private string _MiddleName; 

        public string BirthDate { get; set; }

        public string Email
        {
            get => _Email;
            set => _Email = value?.Trim();
        }
        private string _Email;

        public string Note { get; set; }

        public string Phone { get; set; }

        public string Address
        {
            get => _Address;
            set => _Address = value?.Trim();
        }
        private string _Address;

        public string Post { get; set; }
        public string Sex { get; set; }      
        public int? IsInSheduler { get; set; }
        public bool? IsInArchive { get; set; } = false;
        public int? UseIndividualPrice { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
