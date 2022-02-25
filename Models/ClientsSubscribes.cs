using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("ClientsSubscribes")]
    public class ClientsSubscribes : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set => _Name = value?.Trim();
        }
        private string _Name;

        public string Comment { get; set; }
        public string Content { get; set; }
        public string DateSubscribe { get; set; }

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public int? SubscribeParamsId { get; set; }
        public SubscribeParams SubscribeParams { get; set; }

        public string Status { get; set; }
        public string JsonSettings { get; set; }
        public string JsonReport { get; set; }


        [NotMapped]
        public Services.Smsc.SmsSettings.Settings Settings { get; set; } = new Services.Smsc.SmsSettings.Settings();

        [NotMapped]
        public Services.Smsc.SmsSettings.Report Report { get; set; } = new Services.Smsc.SmsSettings.Report();

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override bool Equals(object other)
        {
            if (other is ClientsSubscribes clone)
            {
                if (object.ReferenceEquals(this, clone)) return true;
                if (
                    StringParamsIsEquel(this.Name, clone.Name) &&
                    StringParamsIsEquel(this.Guid, clone.Guid) &&
                    StringParamsIsEquel(this.Comment, clone.Comment) &&
                    StringParamsIsEquel(this.Content, clone.Content) &&
                    StringParamsIsEquel(this.DateSubscribe, clone.DateSubscribe) &&
                    StringParamsIsEquel(this.Status, clone.Status) &&
                    StringParamsIsEquel(this.JsonSettings, clone.JsonSettings) &&
                    StringParamsIsEquel(this.JsonReport, clone.JsonReport) &&
                    this?.ParentId == clone?.ParentId &&
                    this?.SubscribeParamsId == clone?.SubscribeParamsId
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

        public override string ToString() => Name;

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsDir));
            OnPropertyChanged(nameof(ParentId));
            OnPropertyChanged(nameof(DateSubscribe));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(SubscribeParams));
        }
    }
}
