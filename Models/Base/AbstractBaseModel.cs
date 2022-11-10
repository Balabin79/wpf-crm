using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Dental.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dental.Infrastructures.Logs;
using DevExpress.Mvvm;
using Dental.Infrastructures.Attributes;

namespace Dental.Models.Base
{
    [Serializable]
    abstract public class AbstractBaseModel : BindableBase, IModel
    {
        public AbstractBaseModel()
        {
            try
            {
                if (Id == 0)
                {
                    SetCreatedAt();
                    SetUpdatedAt();
                    SetGuid();
                }
            } catch(Exception e)
            {
                (new ViewModelLog(e)).run();
            }
        }

        [Key]
        [Column("Id")]
        public virtual int Id
        {
            get { return GetProperty(() => Id); }
            set { SetProperty(() => Id, value); }
        }


        public string Guid { get; set; }
        public int? CreatedAt { get; set; }

        [Clonable]
        public int? UpdatedAt 
        {
            get { return GetProperty(() => UpdatedAt); }
            set { SetProperty(() => UpdatedAt, value); }
        }

        private void SetCreatedAt() => CreatedAt = GetUTCTimestamp();
        private void SetUpdatedAt() => UpdatedAt = GetUTCTimestamp();
        private void SetGuid() => Guid = KeyGenerator.GetUniqueKey();
        private int GetUTCTimestamp() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        /*public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }*/

        public virtual void Update() => SetUpdatedAt();
    }
}
