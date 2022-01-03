using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("LocationAppointment")]
    public class LocationAppointment : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name 
        {
            get => name;
            set => name = value?.Trim(); 
        }
        private string name;

        public string Address
        {
            get => address;
            set => address = value?.Trim();
        }
        private string address;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new LocationAppointment
            {
                Id = this.Id,
                Name = this.Name,
                Address = this.Address,
                Guid = this.Guid,
            };
        }

        public LocationAppointment Copy(LocationAppointment model)
        {
            model.Id = this.Id;
            model.Name = this.Name;
            model.Address = this.Address;
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

            return this.Equals(other as LocationAppointment);
        }
        public bool Equals(LocationAppointment other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            StringParamsIsEquel(this.Name, other.Name);
            StringParamsIsEquel(this.Address, other.Address);
            StringParamsIsEquel(this.Guid, other.Guid);
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
