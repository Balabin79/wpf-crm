using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("AppointmentsQueue")]
    public class AppointmentsQueue : AbstractBaseModel
    {
        public int? AppointmentId { get; set; }
        public int? EventTypeId { get; set; } = 0;
        public int? SendingStatusId { get; set; } = 0;
        public string DateTime { get; set; }
    }

    public enum EventType { Added = 0, Edited = 1, Removed = 2};
    public enum SendingStatus { New = 0, Sended = 1, Error = 2};
}
