using Dental.Models.Base;
using Dental.Interfaces;
using DevExpress.Mvvm;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using Dental.Models.PatientCard;
using System.Collections.Generic;

namespace Dental.Models
{
    [Table("CommunicationLog")]
    class Communication : AbstractBaseModel, IDataErrorInfo, ITreeModel, ITreeViewCollection
    {
        [Required(ErrorMessage = @"Поле ""Наименование"" обязательно для заполнения")]
        [MaxLength(255, ErrorMessage = @"Длина не более 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public string Date { get; set; }
        public string Thema { get; set; }
        public string Status { get; set; }
     

        public int? ClientsCategoryId { get; set; }
        public ClientsCategory ClientsCategory { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public string Comment { get; set; }


        public int? ParentId { get; set; }
        public int? IsDir { get; set; }
        public int? IsSys { get; set; }

        public string Error { get => string.Empty; }
        public string this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }
    }
}
