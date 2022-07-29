using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientContactsQueue")]
    public class ClientContactsQueue : AbstractBaseQueue
    {
        public int? ClientId { get; set; }        
        public Client Client { get; set; }        
    }

}
