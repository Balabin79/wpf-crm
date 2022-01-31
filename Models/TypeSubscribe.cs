using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("SubscribeParams")]
    public class SubscribeParams
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public string Param { get; set; }

        public override string ToString() => Name;
    }
}
