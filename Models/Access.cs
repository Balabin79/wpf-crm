using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Accesses")]
    public class Access : AbstractBaseModel
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public override string ToString() => Name;
    }
}
