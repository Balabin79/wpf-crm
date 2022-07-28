using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("GoogleEmployeesContacts")]
    public class GoogleEmployeesContacts : AbstractBaseModel
    {
        public string Name { get; set; }

        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }

        public string EmployeeGuid { get; set; }
        public string ResourceId { get; set; }
    }
}