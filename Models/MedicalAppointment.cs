using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("MedicalAppointment")]
    public class MedicalAppointment : AbstractBaseModel, IDataErrorInfo
    {
        public string PatientName { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; } //DateTime 
        public string EndTime { get; set; } //DateTime 
        public int AppointmentType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public string Location { get; set; }
        public int EmployeeId { get; set; }
        public int LabelId { get; set; }
        public int StatusId { get; set; }
        public int AllDay { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }

    [Table("ResourceEntity")]
    public class ResourceEntity : AbstractBaseModel
    {
        public string Description { get; set; }
    }
}
