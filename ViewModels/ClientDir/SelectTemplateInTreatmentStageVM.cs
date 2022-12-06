using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using Dental.Models.Base;

namespace Dental.ViewModels.ClientDir
{
    public class SelectTemplateInTreatmentStageVM : ViewModelBase, IDataErrorInfo
    {
        public ICollection<TreeTemplate> Templates
        {
            get { return GetProperty(() => Templates); }
            set { SetProperty(() => Templates, value); }
        }

        public string TemplateName
        {
            get { return GetProperty(() => TemplateName); }
            set { SetProperty(() => TemplateName, value); }
        }

        public TreatmentStage Model
        {
            get { return GetProperty(() => Model); }
            set { SetProperty(() => Model, value); }
        }

        public ClientsViewModel VM { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
