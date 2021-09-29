using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientFiles")]
    class ClientFiles : AbstractBaseModel
    {      
        public string Name { get; set; }
        public string Path { get; set; }
        public string DateCreated { get; set; }
        public string Extension { get; set; }
    }
}
