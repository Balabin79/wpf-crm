using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ShedulerStatuses")]
    public class ShedulerStatuses : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Заголовок"" обязательно для заполнения")]
        [Display(Name = "Заголовок")]
        public string Caption
        {
            get => caption;
            set => caption = value?.Trim(); 
        }
        private string caption;

        public string Brush
        {
            get => brush;
            set => brush = value?.Trim();
        }
        private string brush;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new LocationAppointment
            {
                Id = this.Id,
                Name = this.Caption,
                Address = this.Brush,
                Guid = this.Guid,
            };
        }

        public LocationAppointment Copy(LocationAppointment model)
        {
            model.Id = this.Id;
            model.Name = this.Caption;
            model.Address = this.Brush;
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

            StringParamsIsEquel(this.Caption, other.Name);
            StringParamsIsEquel(this.Brush, other.Address);
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
