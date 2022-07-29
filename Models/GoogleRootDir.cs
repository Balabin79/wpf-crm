using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("GoogleRootDir")]
    public class GoogleRootDir : AbstractBaseModel
    {
        public string ResourceId { get; set; }        
        public string LocalDirName { get; set; }
        public int? DirType { get; set; }
        public int? CreateResource { get; set; }
        public int? UseDefaultName { get; set; }
    }

}
