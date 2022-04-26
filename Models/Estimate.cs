using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using DevExpress.Mvvm;

namespace Dental.Models
{
    [Table("Estimates")]
    public class Estimate : AbstractBaseModel, IDataErrorInfo
    {
        [Display(Name = "Название сметы")]
        [Required(ErrorMessage = @"Поле ""Название сметы"" обязательно для заполнения")]
        public string Name { get; set; }

        [Display(Name = "Дата начала")]
        public string StartDate { get; set; }

        public Client Client { get; set; }
        public int ClientId { get; set; }

        public ObservableCollection<EstimateServiceItem> EstimateServiseItems { get; set; } = new ObservableCollection<EstimateServiceItem>();       

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();


        public override bool Equals(object other)
        {
            if (other is Estimate model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if ( StringParamsIsEquel(this.StartDate, model.StartDate) && StringParamsIsEquel(this.Name, model.Name)) return true;
            }
            return false;
        }

        public override int GetHashCode() => Guid.GetHashCode();

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }
    
        public void UpdateFields()
        {
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(Name));
        }
    }
}