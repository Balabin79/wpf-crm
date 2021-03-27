using Dental.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models.PatientCard
{
    [Table("UserQuestionary")]
    class UserQuestionary : AbstractBaseModel
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int QuestionaryId { get; set; }

    }
}
