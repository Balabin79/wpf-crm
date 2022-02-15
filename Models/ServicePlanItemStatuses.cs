using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ServicePlanItemStatuses")]
    public class ServicePlanItemStatuses : AbstractBaseModel
    {
        [Display(Name = "Название")]
        public string Name 
        {
            get => _Name;
            set => _Name = value?.Trim(); 
        }
        private string _Name;

        public object Clone() => this.MemberwiseClone();

        public override string ToString() => Name;

    }
}
