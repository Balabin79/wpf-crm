using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

namespace Dental.Models.Base
{
    public abstract class ModelEntityBase : ViewModelBase
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

    }
}
