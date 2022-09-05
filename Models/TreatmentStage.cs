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

        public string Diagnoses
        {
            get => diagnoses;
            set
            {
                diagnoses = value;
                OnPropertyChanged(nameof(Diagnoses));
            }
        }
        private string diagnoses;

        public string Complaints
        {
            get => complaints;
            set
            {
                complaints = value;
                OnPropertyChanged(nameof(Complaints));
            }
        }
        private string complaints;

        public string Objectively
        {
            get => objectively;
            set
            {
                objectively = value;
                OnPropertyChanged(nameof(Objectively));
            }
        }
        private string objectively;

        public string DescriptionXRay {
            get => descriptionXRay; 
            set
            {
                descriptionXRay = value;
                OnPropertyChanged(nameof(DescriptionXRay));
            } 
        }
        private string descriptionXRay;

        public string Anamneses 
        { 
            get => anamneses;
            set
            {
                anamneses = value;
                OnPropertyChanged(nameof(Anamneses));
            } 
        }
        private string anamneses;

        public string Plans
        {
            get => plans;
            set
            {
                plans = value;
                OnPropertyChanged(nameof(Plans));
            }
        }
        private string plans;

        public string Allergies
        {
            get => allergies;
            set
            {
                allergies = value;
                OnPropertyChanged(nameof(Allergies));
            }
        }
        private string allergies;

        public string Treatments
        {
            get => treatments;
            set
            {
                treatments = value;
                OnPropertyChanged(nameof(Treatments));
            }
        }
        private string treatments;

        public string Recommendations
        {
            get => recommendations;
            set
            {
                recommendations = value;
                OnPropertyChanged(nameof(Recommendations));
            }
        }
        private string recommendations;

        public string Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        private string date;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string name;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();
    }
}
