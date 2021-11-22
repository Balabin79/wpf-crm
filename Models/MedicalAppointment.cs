using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("MedicalAppointment")]
    class MedicalAppointment : AbstractBaseModel, IDataErrorInfo
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int AppointmentType { get; set; }
        public string RecurrenceInfo { get; set; }
        public int ResourceId { get; set; }
        public int Label { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
    public class ResourceEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
