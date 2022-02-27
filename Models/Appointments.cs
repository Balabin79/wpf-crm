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
                if (DateTime.TryParse(StartTime, out DateTime result)) return result;
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
        public int StatusId { get; set; }
        public int AllDay { get; set; }



        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public decimal? KDiscount { get; set; }
        public decimal? KPieceRate { get; set; }
        public decimal? CalcPrice { get; set; }
        public decimal? CalcCost { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }

    [Table("ResourceEntity")]
    public class ResourceEntity : AbstractBaseModel
    {
        public string Description { get; set; }
    }
}