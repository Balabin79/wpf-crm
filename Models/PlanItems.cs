using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dental.Models
{
    [Table("PlanItems")]
    public class PlanItem : AbstractBaseModel
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

        public Plan Plan { get; set; }
        public int? PlanId { get; set; }       

        public int Count 
        {
            get { return GetProperty(() => Count); }
            set { SetProperty(() => Count, value); }
        }

        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public override string ToString() => Name + " (" + Code + ")";

    }
}
