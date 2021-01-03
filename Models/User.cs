using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dental.Infrastructures.Commands.Base;
using Dental.Repositories;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Dental.Models
{


    class User : ViewModelBase
    {
        [Key]
        public int Id { get; set; }
        // Общая информация
        [Display(Name = "Фото")]
        public Image Image { get; set; }

        [Display(Name = "Имя")]
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [MaxLength(255)]
        public string MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Пол")]
        public string Gender { get; set; }

    }

    public enum Gender { Мужчина, Женщина }
}
