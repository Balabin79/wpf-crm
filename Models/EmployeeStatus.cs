using Dental.Infrastructures.Commands.Base;
using Dental.Repositories;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;

namespace Dental.Models
{
    class EmployeeStatus : ViewModelBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }  
    }
}
