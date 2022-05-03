using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("EstimateMaterialItems")]
    public class EstimateMaterialItem : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Материал"" обязательно для заполнения")]
        public Nomenclature Nomenclature
        {
            get => nomenclature;
            set
            {
                nomenclature = value;
                OnPropertyChanged(nameof(Nomenclature));
            }
        }
        public int? NomenclatureId { get; set; }
        private Nomenclature nomenclature;

        public Estimate Estimate { get; set; }
        public int? EstimateId { get; set; }

        public Measure Measure
        {
            get => measure;
            set
            {
                measure = value;
                OnPropertyChanged(nameof(Measure));
            } 
        }
        public int? MeasureId { get; set; }
        private Measure measure;

        public int Count
        {
            get => count;
            set
            {
                count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
        private int count;

        public decimal Price 
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        private decimal price;


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is EstimateMaterialItem model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if (
                    StringParamsIsEquel(this.Guid, model.Guid) &&
                    StringParamsIsEquel(this.Nomenclature?.Guid, model.Nomenclature?.Guid) &&
                    StringParamsIsEquel(this.Estimate?.Guid, model.Estimate?.Guid) &&
                    StringParamsIsEquel(this.Measure?.Guid, model.Measure?.Guid) &&
                    this.Price == model?.Price &&
                    this?.Count == model?.Count
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

    }
}
