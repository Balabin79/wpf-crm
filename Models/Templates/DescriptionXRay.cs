using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Templates
{
    [Table("DescriptionXRay")]
    public class DescriptionXRay : BaseTemplate<DescriptionXRay>
    {
        public override object Clone()
        {
            return new DescriptionXRay()
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
