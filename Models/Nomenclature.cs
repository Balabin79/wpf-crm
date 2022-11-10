using Dental.Infrastructures.Attributes;
using Dental.Models.Base;
using Dental.Models.Templates;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dental.Models
{
    [Serializable]
    [Table("Nomenclature")]
    public class Nomenclature : BaseTemplate<Nomenclature>
    {
        [Display(Name = "Артикул")]
        [Clonable]
        public string Code
        {
            get { return GetProperty(() => Code); }
            set { SetProperty(() => Code, value?.Trim()); }
        }

        [JsonIgnore]
        [NotMapped]
        public string FullName { get => string.IsNullOrEmpty(Code) ? Name : Name + " (Код: " + Code + ")"; }

        [Clonable]
        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public override object Clone()
        {
            return new Nomenclature()
            {
                IsDir = IsDir,
                Name = Name,
                Parent = Parent,
                ParentId = ParentId,
                UpdatedAt = UpdatedAt,
                Code = Code,
                Price = Price
            };
        }

        public override string ToString() => Name;
    }
}
