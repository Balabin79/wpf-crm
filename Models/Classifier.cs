using Dental.Models.Base;
using Dental.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Windows.Input;

namespace Dental.Models
{
    [Table("Classifier")]
    class Classifier : AbstractBaseModel
    {
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public int ParentId { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }


        public bool this[PropertyInfo prop, Classifier item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "ParentId": return item.ParentId == ParentId;
                    case "Description": return item.Description == Description;                 
                    default: return true;
                }
            }
        }

        public void Copy(Classifier copy)
        {
            Name = copy.Name;
            ParentId = copy.ParentId;
            Description = copy.Description;
        }
    }
}
