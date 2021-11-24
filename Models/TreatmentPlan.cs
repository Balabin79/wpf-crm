using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("TreatmentPlans")]
    class TreatmentPlan : AbstractBaseModel, IDataErrorInfo
    {
        public TreatmentPlan()
        {
            TreatmentPlanEmployes = new List<TreatmentPlanEmployes>();
            TreatmentPlanItems = new List<TreatmentPlanItems>();
            Invoices = new List<InvoiceItems>();
        }
        public List<TreatmentPlanEmployes> TreatmentPlanEmployes { get; set; }
        public List<TreatmentPlanItems> TreatmentPlanItems { get; set; }
        public List<InvoiceItems> Invoices { get; set; }

        public int PatientInfoId { get; set; }
        public PatientInfo PatientInfo { get; set; }

        public string Number { get; set; }
        public string Date { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new TreatmentPlan
            {
                Id = this.Id,
                Guid = this.Guid,
                Number = this.Number,
                Date = this.Date,
                PatientInfoId = this.PatientInfoId,
                PatientInfo = this.PatientInfo
            };
        }

        public TreatmentPlan Copy(TreatmentPlan model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.Date = this.Date;
            model.Number = this.Number;
            model.PatientInfoId = this.PatientInfoId;
            model.PatientInfo = this.PatientInfo;
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

            return this.Equals(other as TreatmentPlan);
        }
        public bool Equals(TreatmentPlan other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Guid, other.Guid);
            StringParamsIsEquel(this.Number, other.Number);
            StringParamsIsEquel(this.Date, other.Date);
            StringParamsIsEquel(this.PatientInfo.Guid, other.PatientInfo.Guid);

            if (this.PatientInfoId != other.PatientInfoId) return false;

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
