using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections.ObjectModel;
using Dental.Services.Files;

namespace Dental.Models
{
    [Table("ClientInfo")]
    public class Client : AbstractBaseModel, ICloneable
    {

        public string FirstName
        { 
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            } 
        }
        private string firstName;

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        private string lastName;

        public string MiddleName
        {
            get => middleName;
            set
            {
                middleName = value;
                OnPropertyChanged(nameof(MiddleName));
            }
        }
        private string middleName;

        public string FullName { get => ToString(); }
        public string BirthDate
        {
            get => birthDate;
            set
            {
                birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
            }
        }
        private string birthDate;

        public string Sex
        {
            get => sex;
            set
            {
                sex = value;
                OnPropertyChanged(nameof(Sex));
            }
        }
        private string sex;

        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
        private string phone;

        public string Email
        {
            get => _Email;
            set => _Email = value?.Trim();
        }
        private string _Email;

        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        private string address;

        public string Note { get; set; }
        public bool? IsInArchive { get; set; } = false;
        public bool? IsRemoteContactCreated { get; set; } = false;
        public string PassportSeries { get; set; }
        public string PassportNo { get; set; }
        public string PassportIssuanceDate { get; set; }
        public string WhomIssued { get; set; }
        
        public string GoogleContactGroup { get; set; }
        public string ContactResourceName { get; set; }

        public object Clone() => (Client)this.MemberwiseClone();
      
        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
