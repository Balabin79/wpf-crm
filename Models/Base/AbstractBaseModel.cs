using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using DevExpress.Mvvm;
using B6CRM.Infrastructures.Attributes;
using B6CRM.Services;

namespace B6CRM.Models.Base
{
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
            }
            catch (Exception e)
            {
                Log.ErrorHandler(e);
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

        /*public string Owner { get; set; } = BitConverter.ToString(MD5.Create().
            ComputeHash(new UTF8Encoding().
            GetBytes(Environment.MachineName))).
            Replace("-", string.Empty);*/

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
