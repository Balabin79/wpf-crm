using Dental.Infrastructures.Logs;
using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsRequests")]
    public class ClientsRequests : AbstractBaseModel, IDataErrorInfo
    {
        [Display(Name = "ФИО")]
        public string Fio
        {
            get => fio;
            set => fio = value?.Trim();
        }
        private string fio;

        public string Note { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public PatientInfo ClientInfo { get; set; }
        public int? ClientInfoId { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone()
        {
            try
            {
                return new ClientsRequests
                {
                    Id = this.Id,
                    Fio = this.Fio,
                    Note = this.Note,
                    Date = this.Date,
                    Time = this.Time,
                    Guid = this.Guid,
                    ClientInfo = this.ClientInfo,
                };
            } catch(Exception ex)
            {
                (new ViewModelLog(ex)).run();
                return new ClientsRequests();
            }

        }

        public ClientsRequests Copy(ClientsRequests model)
        {
            model.Id = this.Id;
            model.Fio = this.Fio;
            model.Note = this.Note;
            model.Date = this.Date;
            model.Time = this.Time;
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

            return this.Equals(other as ClientsRequests);
        }
        public bool Equals(ClientsRequests other)
        {
            NotIsChanges = true;
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;           

            StringParamsIsEquel(this.Fio, other.Fio);
            StringParamsIsEquel(this.Note, other.Note);
            StringParamsIsEquel(this.Date, other.Date);
            StringParamsIsEquel(this.Time, other.Time);
            StringParamsIsEquel(this.Guid, other.Guid);
            StringParamsIsEquel(this.ClientInfo?.Guid, other.ClientInfo?.Guid);
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

        public void FieldsUpdate()
        {
            OnPropertyChanged(nameof(Fio));
            OnPropertyChanged(nameof(Note));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Time));
            OnPropertyChanged(nameof(ClientInfo));
        }

    }
}
