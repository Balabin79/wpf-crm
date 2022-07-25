using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Dental.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dental.Infrastructures.Logs;

namespace Dental.Models.Base
{
    abstract public class AbstractBaseQueue :  INotifyPropertyChanged
    {
        public AbstractBaseQueue()
        {
            try
            {
                if (Id == 0)
                {
                    SetCreatedAt();
                    SetGuid();
                }
            } catch(Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Key]
        [Column("Id")]
        public int Id 
        { 
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            } 
        }
        private int id;
        public string Guid { get; set; }
        public int? CreatedAt { get; set; }
        public int? EventTypeId { get; set; } = 0;
        public int? SendingStatusId { get; set; } = 0;


        private void SetCreatedAt() => CreatedAt = GetUTCTimestamp();
        private void SetGuid() => Guid = KeyGenerator.GetUniqueKey();
        private int GetUTCTimestamp() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public enum EventType { Added = 0, Edited = 1, Removed = 2 };
        public enum SendingStatus { New = 0, Sended = 1, Error = 2 };
    }
}
