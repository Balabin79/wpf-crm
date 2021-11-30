using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("TreatmentPlanEmployes")]
    class TreatmentPlanEmployes : AbstractBaseModel, IDataErrorInfo
    {
        public int TreatmentPlanId { get; set; }
        public TreatmentPlanItems TreatmentPlan { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {

            return new TreatmentPlanEmployes
            {
                Id = this.Id,
                Guid = this.Guid,
                TreatmentPlanId = this.TreatmentPlanId,
                EmployeeId = this.EmployeeId,
                TreatmentPlan = this.TreatmentPlan,
                Employee = this.Employee
            };
        }

        public TreatmentPlanEmployes Copy(TreatmentPlanEmployes model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.TreatmentPlanId = this.TreatmentPlanId;
            model.TreatmentPlan = this.TreatmentPlan;
            model.EmployeeId = this.EmployeeId;
            model.Employee = this.Employee;
            return model;
        }


        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            //Если ссылки указывают на один и тот же адрес, то их идентичность гарантирована.
            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as TreatmentPlanEmployes);
        }
        public bool Equals(TreatmentPlanEmployes other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Guid, other.Guid);
            StringParamsIsEquel(this.TreatmentPlan.Guid, other.TreatmentPlan.Guid);
            StringParamsIsEquel(this.Employee.Guid, other.Employee.Guid);

            if (this.TreatmentPlanId != other.TreatmentPlanId) return false;
            if (this.EmployeeId != other.EmployeeId) return false;

            return NotIsChanges;
        }

        private void StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return;
            NotIsChanges = false;
        }

        [NotMapped]
        public bool NotIsChanges { get; set; } = true;
    }
}
