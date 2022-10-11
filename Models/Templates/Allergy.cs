using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Templates
{
    [Table("Allergies")]
    public class Allergy : BaseTemplate<Allergy>
    {
      
        public override object Clone()
        {
            return new Allergy() 
            {
                IsDir = IsDir, 
                Name = Name, 
                Parent = Parent, 
                ParentId = ParentId,
                UpdatedAt = UpdatedAt 
            };
        }
    }
}
