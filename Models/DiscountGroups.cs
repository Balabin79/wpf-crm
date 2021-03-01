using Dental.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dental.Models
{
    [Table("DiscountGroups")]
    class DiscountGroups : AbstractBaseModel
    {
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Вид скидки")]
        public string DiscountGroupType { get; set; } = "Процент";

        [Display(Name = "Размер скидки")]
        public float AmountDiscount { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }


        public bool this[PropertyInfo prop, DiscountGroups item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "Description": return item.Description == Description;                 
                    case "DiscountGroupType": return item.DiscountGroupType == DiscountGroupType;                 
                    case "AmountDiscount": return item.AmountDiscount == AmountDiscount;                 
                    default: return true;
                }
            }
        }

        public void Copy(DiscountGroups copy)
        {
            Name = copy.Name;
            Description = copy.Description;
            DiscountGroupType = copy.DiscountGroupType;
            AmountDiscount = copy.AmountDiscount;
        }
    }
}
