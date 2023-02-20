using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Countries")]
    public class Country : AbstractBaseModel
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
