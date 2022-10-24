using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Appointments")]
    public class Appointments : AbstractBaseModel, IDataErrorInfo
    {
        public string PatientName { get; set; }
        public string Description { get; set; }
        public DateTime Date 
        {
            get 
            {
                if (DateTime.TryParse(StartTime, out DateTime result)) 
                    return result;
                return new DateTime();
            }
        }

        public string StartTime { get; set; } 
        public string EndTime { get; set; } 
        public int AppointmentType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public string LocationName { get; set; }

        public int? ServiceId { get; set; }
        public Service Service { get; set; }

        public int? LocationId { get; set; }
        public LocationAppointment Location { get; set; }

        public int? ClientInfoId { get; set; }
        public Client ClientInfo { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int LabelId { get; set; }

        public int? StatusId { get; set; }
        public AppointmentStatus Status { get; set; }


        public int AllDay { get; set; }

        public string AttachmentFile
        {
            get { return GetProperty(() => AttachmentFile); }
            set { SetProperty(() => AttachmentFile, value?.Trim()); }
        }

        public string AttachmentFileName
        {
            get { return GetProperty(() => AttachmentFileName); }
            set { SetProperty(() => AttachmentFileName, value?.Trim()); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }

}
