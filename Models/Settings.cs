using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Setting : AbstractBaseModel
    {
        public string LoginProdoctorov { get; set; }
        public string PasswordProdoctorov { get; set; }

        public int? IsSingleProfile { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string Password { get; set; }
    }
}
