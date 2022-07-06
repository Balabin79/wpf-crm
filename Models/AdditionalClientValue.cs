using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("AdditionalClientValues")]
    public class AdditionalClientValue : AbstractBaseModel, IDataErrorInfo
    {

        public string Value 
        { 
            get => val;
            set
            {
                val = value;
                OnPropertyChanged(nameof(val));
            }
        }
        private string val;

        public Client Client { get; set; }
        public int? ClientId { get; set; }

        public AdditionalField AdditionalField { get; set; }
        public int? AdditionalFieldId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
    }
}
