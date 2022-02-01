using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("UserActions")]
    public class UserActions : AbstractBaseModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; }              
        public string SectionPage { get; set; }       
        public string SessionGuid { get; set; }       
    }
}
