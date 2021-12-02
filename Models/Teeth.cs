using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Teeth")]
    public class Teeth : AbstractBaseModel, IDataErrorInfo
    {
        public string PatientTeeth { get; set; }

        public int PatientInfoId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new Teeth
            {
                Id = this.Id,
                PatientTeeth = this.PatientTeeth,
                PatientInfoId = this.PatientInfoId,
                Guid = this.Guid,
            };
        }

        public Teeth Copy(Teeth model)
        {
            model.Id = this.Id;
            model.PatientTeeth = this.PatientTeeth;
            model.PatientInfoId = this.PatientInfoId;
            model.Guid = this.Guid;
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

            return this.Equals(other as Teeth);
        }
        public bool Equals(Teeth other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.PatientTeeth, other.PatientTeeth);
            StringParamsIsEquel(this.Guid, other.Guid);
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
