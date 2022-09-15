using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Settings")]
    public class Setting : AbstractBaseModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public int? RolesEnabled { get; set; }
    }
}
