using Dental.Models.Base;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental.Models
{
    [Table("Warehouse")]
    public class Warehouse : AbstractBaseModel, IDataErrorInfo
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name
        {
            get => _Name;
            set 
            {
                _Name = value?.Trim();
            } 
        }
        private string _Name;

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Ответственное лицо")]
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }

        public void UpdateFields()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Employee));
            OnPropertyChanged(nameof(Description));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
