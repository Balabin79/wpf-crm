using B6CRM.Models.Base;
using DevExpress.Mvvm;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
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

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Service Service { get; set; }

        public int? LocationId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public LocationAppointment Location { get; set; }

        public int? ClientInfoId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Client ClientInfo { get; set; }

        public int? EmployeeId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Employee Employee { get; set; }

        public int LabelId { get; set; }

        public int? StatusId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public AppointmentStatus Status { get; set; }


        public int AllDay { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
