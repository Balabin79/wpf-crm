using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("ServicePlans")]
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
        public Client PatientInfo { get; set; }

        public string DateTime
        {
            get =>  (System.DateTime.TryParse(_DateTime, out DateTime result)) ? result.ToShortDateString() : _DateTime;
            set 
            { 
                _DateTime = value;
            }
        }
        private string _DateTime;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is TreatmentPlan clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Name, clone.Name) &&
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.DateTime, clone.DateTime) &&
                    this?.PatientInfo == clone?.PatientInfo

                ) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }

        public void Update()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(DateTime));
        }
    }
}
