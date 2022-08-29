using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("MedicalTab")]
    public class MedicalTab : AbstractBaseModel, IDataErrorInfo
    {
       
        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public string Teeth { get; set; }
        public string Diagnoses { get; set; }
        public string Complaints { get; set; }
        public string Objectively { get; set; }
        public string DescriptionXRay { get; set; }
        public string Anamneses { get; set; }
        public string Plans { get; set; }
        public string Diaries { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }


        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

    }
}
