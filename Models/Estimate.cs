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

        [Display(Name = "Клиент")]
        public Client Client { get; set; }
        public int? ClientId { get; set; }

        [Display(Name = "Дата начала")]
        public string StartDate
        {
            get => (System.DateTime.TryParse(startDate, out DateTime result)) ? result.ToShortDateString() : startDate;
            set
            {
                startDate = value;
            }
        }
        private string startDate;

        [Display(Name = "Дата окончания")]
        public string EndDate
        {
            get => (System.DateTime.TryParse(endDate, out DateTime result)) ? result.ToShortDateString() : endDate;
            set
            {
               endDate = value;
            }
        }
        private string endDate;

        public string Status { get; set; }

        public string Note { get; set; }



        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            var clone = this.MemberwiseClone();
            ((Estimate)clone).Client = this.Client;
            return clone;
        }

        public override bool Equals(object other)
        {
            if (other is Estimate clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Name, clone.Name) &&
                    StringParamsIsEquel(this.Status, clone.Status) &&
                    StringParamsIsEquel(this.StartDate, clone.StartDate) &&
                    StringParamsIsEquel(this.EndDate, clone.EndDate) &&
                    StringParamsIsEquel(this.Note, clone.Note) &&
                    this?.Client == clone?.Client

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
