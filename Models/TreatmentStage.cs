using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("TreatmentStage")]
    public class TreatmentStage : AbstractBaseModel, IDataErrorInfo
    {    
        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public string Teeth { get; set; }

     
        public string Allergies
        {
            get { return GetProperty(() => Allergies); }
            set { SetProperty(() => Allergies, value); }
        }        
              
        public string Recommendations
        {
            get { return GetProperty(() => Recommendations); }
            set { SetProperty(() => Recommendations, value); }
        }

        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }

        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();
    }
}
