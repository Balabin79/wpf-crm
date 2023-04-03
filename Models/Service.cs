using B6CRM.Models.Base;
using B6CRM.Models.Templates;
using B6CRM.Infrastructures.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B6CRM.Models
{
    [Table("Services")]
    public class Service : BaseTemplate<Service>, IInvoiceItem
    {
        public Service() => IsHidden = 0;

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

        public int? IsHidden
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
