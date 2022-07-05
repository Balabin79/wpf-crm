using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.Entity;
using DevExpress.Mvvm.Native;

namespace Dental.ViewModels.AdditionalFields
{
    public class FieldVM : BindableBase, IDataErrorInfo
    {
        public FieldVM(ApplicationContext db, int id = 0)
        {
            Fields = db?.AdditionalField.Where(f => f.IsDir == 1 && f.Id != id).Include(f => f.Parent).Include(f => f.TypeValue)
                .OrderBy(f => f.Caption).ToObservableCollection();
            Templates = db.TemplateType.ToArray();
        }

        [Required(ErrorMessage = @"Поле ""Категория"" обязательно для заполнения")]
        public AdditionalField Parent
        {
            get { return GetProperty(() => Parent); }
            set { SetProperty(() => Parent, value); }
        }
        public int? ParentId { get; set; }

        public int? IsDir { get; set; }
        public int? IsSys { get; set; }

        [Required(ErrorMessage = @"Поле ""Название поля"" обязательно для заполнения")]
        public string Caption
        {
            get { return GetProperty(() => Caption); }
            set { SetProperty(() => Caption, value); }
        }

        [Required(ErrorMessage = @"Поле ""Системное название поля"" обязательно для заполнения")]
        public string SysName
        {
            get { return GetProperty(() => SysName); }
            set { SetProperty(() => SysName, value); }
        }

        public TemplateType TypeValue
        {
            get { return GetProperty(() => TypeValue); }
            set { SetProperty(() => TypeValue, value); }
        }
        public int? TypeValueId { get; set; }

        public string Guid { get; set; }

        public ObservableCollection<AdditionalField> Fields
        {
            get { return GetProperty(() => Fields); }
            set { SetProperty(() => Fields, value); }
        }

        public ICollection<TemplateType> Templates { get; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }

    
}
