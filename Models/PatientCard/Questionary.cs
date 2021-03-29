using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.PatientCard
{
    [Table("Questionary")]
    class Questionary : AbstractBaseModel
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
