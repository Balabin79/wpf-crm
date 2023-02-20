using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dental.Models.Base;
using System.Windows.Media;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Dental.Models
{
    [Table("Employees")]
    public class Employee : AbstractUser, IDataErrorInfo, ICloneable
    {
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
        public bool? IsInArchive { get; set; } = false;

        public int? IsAdmin { get; set; } = 0;
        public int? IsDoctor { get; set; } = 1;
        public int? IsReception { get; set; } = 0;

        public string Password
        {
            get { return GetProperty(() => Password); }
            set { SetProperty(() => Password, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();
    }
}
