using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("EstimateCategories")]
    public class EstimateCategory : AbstractBaseModel, IDataErrorInfo
    {
        [MaxLength(255, ErrorMessage = @"Длина не более 500 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = @"Поле ""Клиент"" обязательно для заполнения")]
        public Client Client { get; set; }
        public int? ClientId { get; set; }

        public ObservableCollection<Estimate> Estimates { get; set; } = new ObservableCollection<Estimate>();

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
        
        public override bool Equals(object other)
        {
            if (other is EstimateCategory model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if (StringParamsIsEquel(this.Name, model.Name) && StringParamsIsEquel(this.Guid, model.Guid)) return true;
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
    }
}
