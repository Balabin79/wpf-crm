using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Base
{
    abstract class AbstractBaseModel : IModel
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
    }
}
