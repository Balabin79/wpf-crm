using B6CRM.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace B6CRM.Models
{
    [Table("InvoiceItems")]
    public class InvoiceItems : AbstractBaseModel
    {
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

        public Invoice Invoice { get; set; }
        public int? InvoiceId { get; set; }

        public int? Count
        {
            get { return GetProperty(() => Count); }
            set { SetProperty(() => Count, value); }
        }

        [Column(TypeName = "NUMERIC")]
        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public override string ToString() => Name + " (" + Code + ")";

    }
}
