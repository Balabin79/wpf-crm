using Dental.Models;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using Dental.Services;
using System.Data.Entity;

namespace Dental.ViewModels.Invoices
{
    public class MaterialVM : BindableBase, IDataErrorInfo
    {
        public MaterialVM(ApplicationContext db, int id = 0) => Materials = db?.Nomenclature.Where(f => f.IsDir == 1 && f.Id != id).Include(f => f.Parent)
            .OrderBy(f => f.Name).ToObservableCollection();

        [Required(ErrorMessage = @"Поле ""Клиент"" обязательно для заполнения")]
        public string Name
        {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

        public string Code
        {
            get { return GetProperty(() => Code); }
            set { SetProperty(() => Code, value); }
        }

        public decimal? Price
        {
            get { return GetProperty(() => Price); }
            set { SetProperty(() => Price, value); }
        }

        public int? IsDir
        {
            get { return GetProperty(() => IsDir); }
            set { SetProperty(() => IsDir, value); }
        }

        public string Guid { get; set; }

        public bool IsVisibleItemForm
        {
            get { return GetProperty(() => IsVisibleItemForm); }
            set { SetProperty(() => IsVisibleItemForm, value); }
        }

        public ObservableCollection<Nomenclature> Materials
        {
            get { return GetProperty(() => Materials); }
            set { SetProperty(() => Materials, value); }
        }

        public Nomenclature Parent { get; set; }
        public int? ParentId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
