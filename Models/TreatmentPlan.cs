
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Interfaces;
using Dental.Models.Base;

namespace Dental.Models
{
    [Table("TreatmentPlan")]
    class TreatmentPlan : TreeModelBase, ITreeViewCollection
    {

    }
}
