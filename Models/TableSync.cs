using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("TablesSync")]
    public class TableSync : AbstractBaseModel
    {
        public string Tables { get; set; }
        public string Hash { get; set; }
    }
}
