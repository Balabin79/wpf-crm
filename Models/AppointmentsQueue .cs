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
    public class AppointmentsQueue : AbstractBaseQueue
    {
        public int? AppointmentId { get; set; }        
        public string DateTime { get; set; }
    }

}
