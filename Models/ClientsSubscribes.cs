using Dental.Infrastructures.Logs;
using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsSubscribes")]
    public class ClientsSubscribes : AbstractBaseModel, IDataErrorInfo
    {
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set => _Name = value?.Trim();
        }
        private string _Name;


        public PatientInfo ClientInfo { get; set; }
        public int? ClientInfoId { get; set; } = 1;


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            try
            {
                return new ClientsSubscribes
                {
                    Id = this.Id,
                    Name = this.Name,
                    Guid = this.Guid,
                    ClientInfo = this.ClientInfo,
                };
            } catch(Exception ex)
            {
                (new ViewModelLog(ex)).run();
                return new ClientsRequests();
            }

        }

        public ClientsSubscribes Copy(ClientsSubscribes model)
        {
            model.Id = this.Id;
            model.Name = this.Name;
            model.Guid = this.Guid;
            model.ClientInfo = this.ClientInfo;
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

            return this.Equals(other as ClientsSubscribes);
        }
        public bool Equals(ClientsSubscribes other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;           

            StringParamsIsEquel(this.Name, other.Name);
            StringParamsIsEquel(this.Guid, other.Guid);
            if (this.ClientInfo != other.ClientInfo) return false;
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
