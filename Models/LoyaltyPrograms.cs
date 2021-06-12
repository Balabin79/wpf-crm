using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Dental.Models
{
    [Table("LoyaltyPrograms")]
    class LoyaltyPrograms : AbstractBaseModel, IDataErrorInfo, ITreeModel, ITreeViewCollection
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Действует до")]
        public string PeriodTo {
            get { 
                try
                {
                    DateTime.TryParse(_PeriodTo, out DateTime v);
                    if (string.IsNullOrEmpty(_PeriodTo)) return DateTime.Now.ToShortDateString().ToString();
                    return v.ToShortDateString().ToString();
                } catch(Exception e)
                {
                    return DateTime.Now.ToShortDateString();
                }
                
            }
            set => _PeriodTo = value;
        }

        public int? ParentId { get; set; }
        public int? IsDir { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
        private string _PeriodTo;
    }
}
