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
using System.Windows.Media;

namespace Dental.Models
{
    [Table("ClientInfo")]
    public class Client : AbstractBaseModel, ICloneable
    {

        public string FirstName
        {
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

        public string FullName { get => ToString(); }

        public string BirthDate
        {
            get { return GetProperty(() => MiddleName); }
            set { SetProperty(() => MiddleName, value); }
        }

        public string Sex
        {
            get { return GetProperty(() => Sex); }
            set { SetProperty(() => Sex, value); }
        }

        public string Phone
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value); }
        }

        public string Email
        {
            get { return GetProperty(() => Phone); }
            set { SetProperty(() => Phone, value?.Trim()); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
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

        public string Teeth { get; set; }
        public string ChildTeeth { get; set; }
        public bool? IsChild { get; set; }

        public string Note { get; set; }
        public bool? IsInArchive { get; set; } = false;
        public string PassportSeries { get; set; }
        public string PassportNo { get; set; }
        public string PassportIssuanceDate { get; set; }
        public string WhomIssued { get; set; }     

        public object Clone() => (Client)this.MemberwiseClone();
      
        public override string ToString() => (LastName + " " + FirstName + " " + MiddleName).Trim(' ');
    }
}
