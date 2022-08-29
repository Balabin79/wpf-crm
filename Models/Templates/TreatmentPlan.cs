using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models.Templates
{
    [Table("TreatmentPlans")]
    public class TreatmentPlan : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
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

        public int? IsDir { get; set; }
        public int? ParentId { get; set; }
        public TreatmentPlan Parent { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(Name));
        }

        public override string ToString()
        {
            return Name;
        }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is TreatmentPlan clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (StringParamsIsEquel(this.Name, clone.Name) && StringParamsIsEquel(this.Guid, clone.Guid)) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }
    }
}
