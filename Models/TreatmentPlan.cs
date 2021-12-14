using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("TreatmentPlans")]
    public class TreatmentPlan : AbstractBaseModel, IDataErrorInfo
    {
        public TreatmentPlan()
        {
            TreatmentPlanItems = new ObservableCollection<TreatmentPlanItems>();
        }

        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set 
            {
                _Name = value?.Trim();
                OnPropertyChanged(nameof(Name));
            } 
        }
        private string _Name;

        public ObservableCollection<TreatmentPlanItems> TreatmentPlanItems 
        { 
            get => _TreatmentPlanItems; 
            set
            {
                _TreatmentPlanItems = value;
                OnPropertyChanged(nameof(TreatmentPlanItems));
            } 
        }
        private ObservableCollection<TreatmentPlanItems> _TreatmentPlanItems;

        public int PatientInfoId { get; set; }
        public PatientInfo PatientInfo { get; set; }

        public string DateTime
        {
            get =>  (System.DateTime.TryParse(_DateTime, out DateTime result)) ? result.ToShortDateString() : _DateTime;
            set 
            { 
                _DateTime = value;
                OnPropertyChanged(nameof(DateTime));
            }
        }
        private string _DateTime;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new TreatmentPlan
            {
                Id = this.Id,
                Guid = this.Guid,
                Name = this.Name,
                DateTime = this.DateTime,
                PatientInfo = this.PatientInfo,
                PatientInfoId = this.PatientInfoId
            };
        }

        public TreatmentPlan Copy(TreatmentPlan model)
        {
            model.Id = this.Id;
            model.Guid = this.Guid;
            model.DateTime = this.DateTime;
            model.Name = this.Name;
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
            StringParamsIsEquel(this.Name, other.Name);
            StringParamsIsEquel(this.DateTime, other.DateTime);
            StringParamsIsEquel(this?.PatientInfo?.Guid, other?.PatientInfo?.Guid);

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
