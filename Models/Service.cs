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
    [Table("Services")]
    public class Service : BaseTemplate<Service>, IInvoiceItem
    {
        public Service() => IsHidden = false;

       [Display(Name = "Код")]
        [Clonable]
        public string Code
        {
            get { return GetProperty(() => Code); }
            set { SetProperty(() => Code, value?.Trim()); }
        }

        [NotMapped]
        public string FullName { get => string.IsNullOrEmpty(Code) ? Name : Name + " (Код: " + Code + ")"; }

        [Clonable]
        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public bool? IsHidden
        {
            get { return GetProperty(() => IsHidden); }
            set { SetProperty(() => IsHidden, value); }
        }

        public int? Sort
        {
            get { return GetProperty(() => Sort); }
            set { SetProperty(() => Sort, value); }
        }

        [NotMapped]
        public bool Print
        {
            get { return GetProperty(() => Print); }
            set { SetProperty(() => Print, value); }
        }

        public override object Clone()
        {
            return new Service()
            {
                IsDir = IsDir,
                Name = Name,
                Parent = Parent,
                ParentID = ParentID,
                UpdatedAt = UpdatedAt,
                Code = Code,
                Price = Price,
                IsHidden = IsHidden,
                Sort = Sort,
                Print = Print
            };
        }
    }
}
