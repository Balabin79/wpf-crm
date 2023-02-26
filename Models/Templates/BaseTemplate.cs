using Dental.Infrastructures.Attributes;
using Dental.Models.Base;
using DevExpress.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dental.Models.Templates
{
    public abstract class BaseTemplate<T> : AbstractBaseModel, IDataErrorInfo, ITree, IModel, ICloneable
    {
        [Display(Name = "Название")]
        [Clonable]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value?.Trim()); }
        }

        [Clonable]
        public int? IsDir
        {
            get { return GetProperty(() => IsDir); }
            set { SetProperty(() => IsDir, value); }
        }

        [Clonable]
        public T Parent
        {
            get { return GetProperty(() => Parent); }
            set { SetProperty(() => Parent, value); }
        }

        [Clonable]
        public int? ParentId
        {
            get { return GetProperty(() => ParentId); }
            set { SetProperty(() => ParentId, value); }
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public override string ToString() => Name;

        public abstract object Clone();
    }
}
