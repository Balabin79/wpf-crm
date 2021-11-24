using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("TreatmentPlanItems")]
    class TreatmentPlanItems : AbstractBaseModel, IDataErrorInfo
    {
        public TreatmentPlanItems()
        {
            TreatmentPlanEmployes = new ObservableCollection<TreatmentPlanEmployes>();
        }

        public int? ClassificatorId { get; set; }
        public Classificator Classificator { get; set; }

        public ICollection TreatmentPlanEmployes { get; set; }
        
        public int? Count { get; set; }

        public string Teeth { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new TreatmentPlanItems
            {
                Id = this.Id,
                Guid = this.Guid,
                ClassificatorId = this.ClassificatorId,
                Count = this.Count,
                Teeth = this.Teeth,
                Price = this.Price,
                Status = this.Status,
                TreatmentPlanEmployes = this.TreatmentPlanEmployes
            };
        }

        public TreatmentPlanItems Copy(TreatmentPlanItems model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.ClassificatorId = this.ClassificatorId;
            model.Count = this.Count;
            model.Teeth = this.Teeth;
            model.Price = this.Price;
            model.Status = this.Status;
            model.TreatmentPlanEmployes = this.TreatmentPlanEmployes;
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

            return this.Equals(other as TreatmentPlanItems);
        }
        public bool Equals(TreatmentPlanItems other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Teeth, other.Teeth);
            StringParamsIsEquel(this.Price, other.Price);
            StringParamsIsEquel(this.Status, other.Status);
            StringParamsIsEquel(this.Guid, other.Guid);        

            if (this.ClassificatorId != other.ClassificatorId) return false;

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
