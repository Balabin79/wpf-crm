using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Templates
{
    [Serializable]
    [Table("Objectively")]
    public class Objectively : BaseTemplate<Objectively>
    {
        public override object Clone()
        {
            return new Objectively()
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
