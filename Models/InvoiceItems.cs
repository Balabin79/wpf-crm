using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("InvoiceItems")]
    class InvoiceItems : AbstractBaseModel, IDataErrorInfo
    {
        public int TreatmentPlanId { get; set; }
        public TreatmentPlan TreatmentPlan { get; set; }

        public int PositionId { get; set; }
        public Classificator Position { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public string Number{ get; set; }
        public string DateTime{ get; set; }
        public string PositionName { get; set; } 
        public string Sum { get; set; } 
        public string Status { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new InvoiceItems
            {
                Id = this.Id,
                Guid = this.Guid,
                TreatmentPlanId = this.TreatmentPlanId,
                TreatmentPlan = this.TreatmentPlan,
                PositionId = this.PositionId,
                Position = this.Position,
                EmployeeId = this.EmployeeId,
                Employee = this.Employee,
                Number = this.Number,
                DateTime = this.DateTime,
                PositionName = this.PositionName,
                Sum = this.Sum,
                Status = this.Status
            };
        }

        public InvoiceItems Copy(InvoiceItems model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.TreatmentPlan = this.TreatmentPlan;
            model.TreatmentPlanId = this.TreatmentPlanId;
            model.PositionId = this.PositionId;
            model.Position = this.Position;
            model.EmployeeId = this.EmployeeId;
            model.Employee = this.Employee;
            model.Number = this.Number;
            model.DateTime = this.DateTime;
            model.PositionName = this.PositionName;
            model.Sum = this.Sum;
            model.Status = this.Status;
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

            return this.Equals(other as InvoiceItems);
        }
        public bool Equals(InvoiceItems other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Sum, other.Sum);
            StringParamsIsEquel(this.Status, other.Status);
            StringParamsIsEquel(this.DateTime, other.DateTime);
            StringParamsIsEquel(this.PositionName, other.PositionName);
            StringParamsIsEquel(this.Number, other.Number);
            StringParamsIsEquel(this.Guid, other.Guid);
            StringParamsIsEquel(this.Position.Guid, other.Position.Guid);
            StringParamsIsEquel(this.Employee.Guid, other.Employee.Guid);
            StringParamsIsEquel(this.TreatmentPlan.Guid, other.TreatmentPlan.Guid);

            if (this.TreatmentPlanId != other.TreatmentPlanId) return false;
            if (this.PositionId != other.PositionId) return false;
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
