using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("EmployeeContactsQueue")]
    public class EmployeeContactsQueue : AbstractBaseQueue
    {
        public int? EmployeeId { get; set; }        
    }

}
