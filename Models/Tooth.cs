using DevExpress.Mvvm;
using System.ComponentModel;

namespace Dental.Models
{
    public class Tooth : ViewModelBase, IDataErrorInfo
    {
        public string Abbr
        {
            get => _Abbr;
            set => SetValue(ref _Abbr, value);
        }

        public string ToothImagePath 
        { 
            get => _ToothImagePath; 
            set => SetValue(ref _ToothImagePath, value); 
        }

        public int ToothNumber
        {
            get => _ToothNumber;
            set => SetValue(ref _ToothNumber, value);
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        private string _Abbr;
        private string _ToothImagePath;
        private int _ToothNumber;
    }
}
