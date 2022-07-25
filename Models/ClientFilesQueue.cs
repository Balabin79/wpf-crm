using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientFilesQueue")]
    public class ClientFilesQueue : AbstractBaseQueue
    {
        public int? ClientId { get; set; }        
        public string File { get; set; }        
    }

}
