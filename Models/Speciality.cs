using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Dental.Models.Base;
using System.Reflection;
using Dental.Interfaces;

namespace Dental.Models
{
    [Table("Specialities")]
    class Speciality : TreeModelBase, ITreeViewCollection
    {
        [Display(Name = "В расписании")]
        public int? ShowInShedule { get; set; }

        [NotMapped]
        public bool IsEnabledShowInSheduleField { get => Dir != 1; }

        public bool this[PropertyInfo prop, Speciality item]
        {
            get
            {
                switch (prop.Name)
                {
                    case "Id": return item.Id == Id;
                    case "Name": return item.Name == Name;
                    case "ParentId": return item.ParentId == ParentId;
                    case "ShowInShedule": return item.ShowInShedule == ShowInShedule;
                    default: return true;
                }
            }
        }

        public void Copy(Speciality copy)
        {
            Name = copy.Name;
            ShowInShedule = copy.ShowInShedule;
            ParentId = copy.ParentId;
            Dir = copy.Dir;
            IsSys = copy.IsSys;
            IsDelete = copy.IsDelete;
        }
    }
}
