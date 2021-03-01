using Dental.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("Specialities")]
    class Speciality : AbstractBaseModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name {get; set; }

        [Display(Name = "В расписании")]
        public int ShowInShedule { get; set; }


        public bool this[PropertyInfo prop, Speciality item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "ShowInShedule": return item.ShowInShedule == ShowInShedule;                 
                    default: return true;
                }
            }
        }

        public void Copy(Speciality copy)
        {
            Name = copy.Name;
            ShowInShedule = copy.ShowInShedule;
        }
    }
}
