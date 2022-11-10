using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Serializable]
    [Table("TreatmentStage")]
    public class TreatmentStage : AbstractBaseModel, IDataErrorInfo
    {    
        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public string Teeth { get; set; }

        public string Diagnoses
        {
            get { return GetProperty(() => Diagnoses); }
            set { SetProperty(() => Diagnoses, value); }
        }

        public string Complaints
        {
            get { return GetProperty(() => Complaints); }
            set { SetProperty(() => Complaints, value); }
        }

        public string Objectively
        {
            get { return GetProperty(() => Objectively); }
            set { SetProperty(() => Objectively, value); }
        }        
        
        public string DescriptionXRay
        {
            get { return GetProperty(() => DescriptionXRay); }
            set { SetProperty(() => DescriptionXRay, value); }
        }        
        
        public string Anamneses
        {
            get { return GetProperty(() => Anamneses); }
            set { SetProperty(() => Anamneses, value); }
        }        
        
        public string Plans
        {
            get { return GetProperty(() => Plans); }
            set { SetProperty(() => Plans, value); }
        }        
        
        public string Allergies
        {
            get { return GetProperty(() => Allergies); }
            set { SetProperty(() => Allergies, value); }
        }        
        
        public string Treatments
        {
            get { return GetProperty(() => Treatments); }
            set { SetProperty(() => Treatments, value); }
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
