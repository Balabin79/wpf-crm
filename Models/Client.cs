using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.EntityFrameworkCore;

namespace B6CRM.Models
{
    [Table("ClientInfo")]
    public class Client : AbstractUser, ICloneable
    {

        string _BirthDate;
        public string BirthDate
        {
            get { return DateTime.TryParse(_BirthDate?.ToString(), out DateTime dateTime) ? dateTime.ToShortDateString() : _BirthDate; }
            set { SetValue(ref _BirthDate, value, changedCallback: () => RaisePropertyChanged(nameof(BirthDate))); }
        }

        public string Gender
        {
            get { return GetProperty(() => Gender); }
            set { SetProperty(() => Gender, value); }
        }

        public string Address
        {
            get { return GetProperty(() => Address); }
            set { SetProperty(() => Address, value?.Trim()); }
        }

        public string Note { get; set; }
        public int? IsInArchive { get; set; } = 0;

        [DeleteBehavior(DeleteBehavior.SetNull)]
        public ClientCategory ClientCategory
        {
            get { return GetProperty(() => ClientCategory); }
            set { SetProperty(() => ClientCategory, value); }
        }

        public int? ClientCategoryId
        {
            get { return GetProperty(() => ClientCategoryId); }
            set { SetProperty(() => ClientCategoryId, value); }
        }

        [NotMapped]
        public bool Print
        {
            get { return GetProperty(() => Print); }
            set { SetProperty(() => Print, value); }
        }
        public object Clone() => (Client)MemberwiseClone();
    }
}
