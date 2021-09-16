using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientTreatmentPlans")]
    class ClientTreatmentPlans : AbstractBaseModel, IDataErrorInfo
        {
            [Required(ErrorMessage = @"Поле ""Номер"" обязательно для заполнения")]
            [Display(Name = "Номер")]
            public string TreatmentPlanNumber { get; set; }
            public string Date { get; set; }

            public int PatientInfoId { get; set; }
            public PatientInfo PatientInfo { get; set; }

            public string Error { get => string.Empty; }
            public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
        }
    }
