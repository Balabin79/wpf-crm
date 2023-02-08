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

        string _BirthDate;
        public string BirthDate
        {
            get { return DateTime.TryParse(_BirthDate?.ToString(), out DateTime dateTime) ? dateTime.ToShortDateString() : _BirthDate; }
            set { SetValue(ref _BirthDate, value, changedCallback: () => RaisePropertyChanged(nameof(BirthDate))); }
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

        string _PassportIssuanceDate;
        public string PassportIssuanceDate
        {
            get { return DateTime.TryParse(_PassportIssuanceDate?.ToString(), out DateTime dateTime) ? dateTime.ToShortDateString() : _PassportIssuanceDate; }
            set { SetValue(ref _PassportIssuanceDate, value, changedCallback: () => RaisePropertyChanged(nameof(PassportIssuanceDate))); }
        }

        public string WhomIssued { get; set; }     

        public object Clone() => (Client)this.MemberwiseClone();
    }
}
