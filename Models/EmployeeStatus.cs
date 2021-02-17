using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("EmployeeStatus")]
    class EmployeeStatus : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public bool this[PropertyInfo prop, EmployeeStatus item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "Description": return item.Description == Description;
                    default: return true;
                }
            }
        }

        public void Copy(EmployeeStatus copy)
        {
            Id = copy.Id;
            Name = copy.Name;
            Description = copy.Description;
        }
    }
}
