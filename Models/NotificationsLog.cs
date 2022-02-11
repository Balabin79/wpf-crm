using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("NotificationsLog")]
    public class NotificationsLog : AbstractBaseModel
    {
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public string UserName { get; set; }
        public string Type { get; set; } //phone, email
        public string Contact { get; set; }
        public string Content { get; set; }

        [NotMapped]
        public string Date
        {
            get
            {
                if (CreatedAt == null || !double.TryParse(CreatedAt.ToString(), out double result)) return "";
                return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(result).ToShortDateString();
            }
        }
    }
}
