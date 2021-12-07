using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("TreatmentPlansItems")]
    public class TreatmentPlanItems : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {    
        [Required(ErrorMessage = @"Поле ""Классификатор"" обязательно для заполнения")]
        public Classificator Classificator 
        {
            get => _Classificator;
            set 
            {
                _Classificator = value;
                OnPropertyChanged(nameof(Classificator));
            } 
        }
        public int? ClassificatorId { get; set; }

        private Classificator _Classificator;

        public int TreatmentPlanId { get; set; }
        public TreatmentPlan TreatmentPlan { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int Count { get; set; } = 1;

        public string Note { get; set; }
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
                Note = this.Note,
                Price = this.Price,
                Status = this.Status,
                TreatmentPlanId = this.TreatmentPlanId,
                TreatmentPlan = this.TreatmentPlan,
                Employee = this.Employee
            };
        }

        public TreatmentPlanItems Copy(TreatmentPlanItems model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.ClassificatorId = this.ClassificatorId;
            model.Count = this.Count;
            model.Note = this.Note;
            model.Price = this.Price;
            model.Status = this.Status;
            model.TreatmentPlanId = this.TreatmentPlanId;
            model.TreatmentPlan = this.TreatmentPlan;
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

            StringParamsIsEquel(this.Note, other.Note);
            StringParamsIsEquel(this.Price, other.Price);
            StringParamsIsEquel(this.Status, other.Status);
            StringParamsIsEquel(this.Guid, other.Guid);        
            StringParamsIsEquel(this?.TreatmentPlan?.Guid, other?.TreatmentPlan?.Guid);        
            StringParamsIsEquel(this?.Classificator?.Guid, other?.Classificator?.Guid);        
            StringParamsIsEquel(this?.Employee?.Guid, other?.Employee?.Guid);        

            if (this.ClassificatorId != other.ClassificatorId) return false;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
