using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;

namespace Dental.ViewModels.ClientDir
{
    public class TreatmentStageVM : ViewModelBase, IDataErrorInfo
    {
        public delegate void SaveCommand(object m);
        public event SaveCommand EventSave;

        public string Date
        {
            get { return GetProperty(() => Date); }
            set { SetProperty(() => Date, value); }
        }

        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public TreatmentStage Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        [Command]
        public void Save(object p)
        {
            try
            {
                Model.Name = Name;
                Model.Date = Date;
                EventSave?.Invoke(Model);
                if (p is Window win) win?.Close();
            }
            catch
            {
                if (p is Window win) win?.Close();
            }
        }

    }
}
