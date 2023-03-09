using Dental.Models.Base;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace Dental.Models
{
    [Table("PlanStatuses")]
    public class PlanStatus : AbstractBaseModel, IDataErrorInfo
    {
        public string Name { get; set; }

        public int? Sort { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
        public override string ToString() => Name;

    }
}
