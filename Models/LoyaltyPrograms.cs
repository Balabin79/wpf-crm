using Dental.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("LoyaltyPrograms")]
    class LoyaltyPrograms : AbstractBaseModel
    {
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Действует до")]
        public string PeriodTo { get; set; }


        public bool this[PropertyInfo prop, LoyaltyPrograms item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "Description": return item.Description == Description;                 
                    case "PeriodTo": return item.PeriodTo == PeriodTo;                 
                    default: return true;
                }
            }
        }

        public void Copy(LoyaltyPrograms copy)
        {
            Name = copy.Name;
            Description = copy.Description;
            PeriodTo = copy.PeriodTo;
        }
    }
}
