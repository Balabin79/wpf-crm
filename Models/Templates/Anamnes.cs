using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Templates
{
    [Serializable]
    [Table("Anamnes")]
    public class Anamnes : BaseTemplate<Anamnes>
    {
        public override object Clone()
        {
            return new Anamnes()
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
