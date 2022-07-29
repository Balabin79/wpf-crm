using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("GoogleClientsContacts")]
    public class GoogleClientsContacts : AbstractBaseModel
    {
        public string Name { get; set; }

        public Client Client { get; set; }
        public int? ClientId { get; set; }

        public string ClientGuid { get; set; }
        public string ResourceId { get; set; }
        public string ContactGroupId { get; set; }
    }
}
