using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental.Models
{
    public class Tooth : ViewModelBase, IDataErrorInfo
    {
        public string Abbr
        {
            get { return GetProperty(() => Abbr); }
            set { SetProperty(() => Abbr, value); }
        }

        public string ToothImagePath
        {
            get { return GetProperty(() => ToothImagePath); }
            set { SetProperty(() => ToothImagePath, value); }
        }

        public int ToothNumber
        {
            get { return GetProperty(() => ToothNumber); }
            set { SetProperty(() => ToothNumber, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
