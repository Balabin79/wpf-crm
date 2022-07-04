using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("AdditionalField")]
    public class AdditionalField : AbstractBaseModel, IDataErrorInfo, ITreeModel
    {

        public string Caption 
        { 
            get => caption;
            set
            {
                caption = value;
                OnPropertyChanged(nameof(caption));
            }
        }
        private string caption;

        public string SysName
        {
            get => sysName;
            set
            {
                sysName = value;
                OnPropertyChanged(nameof(sysName));
            }
        }
        private string sysName;

        public string TypeValue
        {
            get => typeValue;
            set
            {
                typeValue = value;
                OnPropertyChanged(nameof(typeValue));
            }
        }
        private string typeValue;

        public AdditionalField Parent
        {
            get => parent;
            set
            {
                parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }
        private AdditionalField parent;
        public int? ParentId { get; set; }

        public int? IsDir { get; set; }
        public int? IsSys { get; set; } = 0;

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();
    }
}
