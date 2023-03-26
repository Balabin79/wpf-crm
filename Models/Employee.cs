using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace Dental.Models
{
    [Table("Employees")]
    public class Employee : AbstractUser, IDataErrorInfo, ICloneable
    {
        public Employee() => IsNotify = 0;

        [NotMapped]
        public bool IsVisible
        {
            get { return GetProperty(() => IsVisible); }
            set { SetProperty(() => IsVisible, value); }
        }

        [NotMapped]
        public ImageSource Image
        {
            get { return GetProperty(() => Image); }
            set { SetProperty(() => Image, value); }
        }

        [NotMapped]
        public string Fio
        {
            get => ToString();
        }

        public string Note { get; set; }

        public string Post
        {
            get { return GetProperty(() => Post); }
            set { SetProperty(() => Post, (value ?? "").Trim()); }
        }

        public int? IsInSheduler { get; set; } = 1;
        public int? IsInArchive { get; set; } = 0;

        public int? IsAdmin { get; set; } = 0;
        public int? IsDoctor { get; set; } = 1;
        public int? IsReception { get; set; } = 0;

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value); }
        }

        public int? IsNotify
        {
            get { return GetProperty(() => IsNotify); }
            set { SetProperty(() => IsNotify, value); }
        }

        public string Telegram
        {
            get { return GetProperty(() => Telegram); }
            set { SetProperty(() => Telegram, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();
    }
}
