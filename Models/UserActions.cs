using Dental.Models.Base;
using System;
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

        [NotMapped]
        public string Date
        {
            get
            {
                if (CreatedAt == null || !double.TryParse(CreatedAt.ToString(), out double result)) return "";
                return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(result).ToShortDateString();
            }
        }
    }
}
