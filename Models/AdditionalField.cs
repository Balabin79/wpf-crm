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
    public class AdditionalField : AbstractBaseModel, IDataErrorInfo, INotifyPropertyChanged
    {
        public AdditionalField()
        {
            //InvoiceServiceItems = new ObservableCollection<InvoiceServiceItems>();
        }

        public AdditionalFieldsCategory AdditionalFieldsCategory
        {
            get => additionalFieldsCategory;
            set
            {
                additionalFieldsCategory = value;
                OnPropertyChanged(nameof(caption));
            }
        }
        public int? additionalFieldsCategoryId;
        private AdditionalFieldsCategory additionalFieldsCategory;

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

        //public ObservableCollection<InvoiceMaterialItems> InvoiceMaterialItems { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => Guid.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is AdditionalField model)
            {
                if (object.ReferenceEquals(this, model)) return true;
                if (
                    StringParamsIsEquel(Caption, model.Caption) && 
                    StringParamsIsEquel(AdditionalFieldsCategory?.Guid, model.AdditionalFieldsCategory?.Guid) && 
                    StringParamsIsEquel(TypeValue, model.TypeValue) && 
                    StringParamsIsEquel(SysName, model.SysName) 
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
    }
}
