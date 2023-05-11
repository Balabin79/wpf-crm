using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("Sms")]
    public class Sms : AbstractBaseModel
    {
        public string Date { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Provider Provider { get; set; }
        public int? ProviderId { get; set;}

        public string CategoryName { get; set; }
        public string SendingStatus { get; set; }
    }
}
