using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Branches")]
    public class Branch : AbstractBaseModel
    {
        public string WorkTime { get; set; }
        public int? BranchId { get; set; }
    }
}