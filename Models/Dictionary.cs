using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Dictionary")]
    public class Dictionary : AbstractBaseModel, IDataErrorInfo
    {
        public string Name 
        {
            get => _Name;
            set => _Name = value?.Trim(); 
        }
        private string _Name;

        public int CategoryId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override string ToString() => Name;        
    }
}
