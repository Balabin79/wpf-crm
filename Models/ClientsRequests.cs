using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsRequests")]
    public class ClientsRequests : AbstractBaseModel, IDataErrorInfo
    {
        public string Contacts { get; set; }
        public string Note { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public PatientInfo ClientInfo { get; set; }
        public int? ClientInfoId { get; set; }


        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is ClientsRequests clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Contacts, clone.Contacts) && 
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.Note, clone.Note) && 
                    StringParamsIsEquel(this.Date, clone.Date) &&
                    StringParamsIsEquel(this.Time, clone.Time) &&
                    StringParamsIsEquel(this.ClientInfo?.Guid, clone.ClientInfo?.Guid)
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

        public override string ToString() => Note;
    }
}
