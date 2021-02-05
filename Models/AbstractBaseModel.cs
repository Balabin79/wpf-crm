using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    abstract class AbstractBaseModel
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
    }
}
