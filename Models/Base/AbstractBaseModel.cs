using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Dental.Models.Base
{
    abstract public class AbstractBaseModel :  IModel
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        public string Guid { get; set; }
    }
}
