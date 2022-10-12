using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Reestr")]
    public class Reestr
    {
        public int Id { get; set; }
        public int Table { get; set; } = 0;
        public int IsSynchronized { get; set; } = 0;
        public int UpdatedAt { get; set; } = 0;
    }
}
