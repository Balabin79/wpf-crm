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
       // public Client() => Estimates = new ObservableCollection<Estimate>();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get => ToString(); }
        public string BirthDate { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public bool? IsInArchive { get; set; } = false;
        public string PassportSeries { get; set; }
        public string PassportNo { get; set; }
        public string PassportIssuanceDate { get; set; }
        public string WhomIssued { get; set; }
     //   public ObservableCollection<Estimate> Estimates { get; set; }

        public object Clone() => (Client)this.MemberwiseClone();
      
        public override string ToString() => LastName + " " + FirstName + " " + MiddleName;
    }
}
