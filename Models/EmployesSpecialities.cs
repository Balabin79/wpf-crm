using Dental.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using DevExpress.Mvvm;
namespace Dental.Models
{
    [Table("EmployesSpecialities")]
    public class EmployesSpecialities : AbstractBaseModel, IDataErrorInfo
    {
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? SpecialityId { get; set; }
        public Speciality Speciality { get; set; }

        public string EmployeeGuid { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
