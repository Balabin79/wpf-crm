using B6CRM.Models.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace B6CRM.Models
{
    [Table("PlanItems")]
    public class PlanItem : AbstractBaseModel
    {
        public PlanItem() => IsInInvoice = 0;

        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Code
        {
            get { return GetProperty(() => Code); }
            set { SetProperty(() => Code, value); }
        }

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Plan Plan { get; set; }

        public int? PlanId { get; set; }

        public int? Count
        {
            get { return GetProperty(() => Count); }
            set { SetProperty(() => Count, value); }
        }

        public string VisitDate
        {
            get { return GetProperty(() => VisitDate); }
            set { SetProperty(() => VisitDate, value); }
        }

        public int? IsInInvoice
        {
            get { return GetProperty(() => IsInInvoice); }
            set { SetProperty(() => IsInInvoice, value); }
        }

        public int? IsMovedToInvoice { get; set; } = 0;

        [Column(TypeName = "NUMERIC")]
        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public override string ToString() => Name + " (" + Code + ")";

    }
}
