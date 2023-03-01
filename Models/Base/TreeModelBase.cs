using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dental.Models.Base
{
    abstract class TreeModelBase : AbstractBaseModel, ITree, IModel, IDataErrorInfo
    {
        [Column("ParentID")]
        public int? ParentID { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("IsDir")]
        [Display(Name = "Директория")]
        public int? IsDir { get; set; }

        [Column("IsSys")]
        [Display(Name = "Системные значения")]
        [NotMapped]
        public int? IsSys { get; set; }

        [Column("IsDelete")]
        [Display(Name = "Удалено")]
        [NotMapped]
        public int? IsDelete { get; set; }

        [NotMapped]
        public bool IsExpanded { 
            get {
                if (Id == 1) { _isExpanded = true; }
                return _isExpanded;
            } 

            //set => Set(ref _isExpanded, value);
        }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        private bool _isExpanded = false;

    }
}
