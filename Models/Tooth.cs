using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    /*[Table("Price")]*/
    public class Tooth// : AbstractBaseModel, IDataErrorInfo
    {

        public string Abbr { get; set; }
        public string ToothImagePath { get; set; }
        public int ToothNumber { get; set; }

        //public string Error { get => string.Empty; }
        //public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
