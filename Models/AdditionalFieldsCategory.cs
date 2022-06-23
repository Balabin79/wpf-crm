using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("AdditionalFieldsCategory")]
    public class AdditionalFieldsCategory : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {
        public AdditionalFieldsCategory()
        {
            //InvoiceServiceItems = new ObservableCollection<InvoiceServiceItems>();
        }

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

        //public ObservableCollection<InvoiceMaterialItems> InvoiceMaterialItems { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is AdditionalFieldsCategory model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if ( StringParamsIsEquel(Caption, model.Caption) && StringParamsIsEquel(SysName, model.SysName) ) return true;
            }
            return false;
        }

        private bool StringParamsIsEquel(string param1, string param2)
        {
            if (string.IsNullOrEmpty(param1) && string.IsNullOrEmpty(param2)) return true;
            if (string.Compare(param1, param2, StringComparison.CurrentCulture) == 0) return true;
            return false;
        }
    }
}
