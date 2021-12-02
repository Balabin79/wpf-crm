using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Dictionary")]
    public class Dictionary : AbstractBaseModel, IDataErrorInfo
    {
        public string Name 
        {
            get => _Name;
            set => _Name = value?.Trim(); 
        }
        private string _Name;

        public int CategoryId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            return new Dictionary
            {
                Id = this.Id,
                Name = this.Name,
                CategoryId = this.CategoryId,
                Guid = this.Guid,
            };
        }

        public Dictionary Copy(Dictionary model)
        {
            model.Id = this.Id;
            model.Name = this.Name;
            model.Guid = this.Guid;
            model.CategoryId = this.CategoryId;
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

            return this.Equals(other as Dictionary);
        }
        public bool Equals(Dictionary other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            if (this.CategoryId != other.CategoryId) return false;

            StringParamsIsEquel(this.Name, other.Name);
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
