using Dental.Models.Base;
using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Windows.Input;

namespace Dental.Models
{
    [Table("Roles")]
    class Role : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }


        public bool this[PropertyInfo prop, Role item]
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

        public void Copy(Role copy, string postfix = " Копия")
        {
            Id = copy.Id;
            Name = (postfix.Length < 1) ? copy.Name : copy.Name + postfix;
            Description = copy.Description;
        }
    }
}
