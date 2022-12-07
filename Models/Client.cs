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
using System.Text.Json.Serialization;

namespace Dental.Models
{
    [Table("ClientInfo")]
    public class Client : AbstractUser, ICloneable
    {
        public string BirthDate
        {
            get { return GetProperty(() => BirthDate); }
            set { SetProperty(() => BirthDate, value); }
        }

        public string Gender
        {
            get { return GetProperty(() => Gender); }
            set { SetProperty(() => Gender, value); }
        }

        public string Snils
        {
            get { return GetProperty(() => Snils); }
            set { SetProperty(() => Snils, value?.Trim()); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
        }

        public string Teeth { get; set; }
        public string ChildTeeth { get; set; }
        public int? IsChild { get; set; }

        public string Note { get; set; }
        public bool? IsInArchive { get; set; } = false;
        public string PassportSeries { get; set; }
        public string PassportNo { get; set; }
        public string PassportIssuanceDate { get; set; }
        public string WhomIssued { get; set; }     

        public object Clone() => (Client)this.MemberwiseClone();
    }
}
