using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Reestr")]
    public class Reestr : AbstractBaseModel
    {
        public string Table { get; set; }
        public int IsSynchronized { get; set; } = 0;
    }
}
