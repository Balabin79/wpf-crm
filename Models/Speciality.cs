using Dental.Infrastructures.Commands.Base;
using Dental.Repositories;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Dental.Models
{
    class Speciality : ViewModelBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Тип персонала")]
        public string SpecialityType { get; set; }

        public bool ShowInShedule { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [NotMapped]
        public List<string> SpecialityTypeList { get => new List<string> { "Младший персонал", "Старший персонал" }; }


        private ObservableCollection<Speciality> listSpecialies;
        public ObservableCollection<Speciality> ListSpecialities
        {
            get
            {
                if (listSpecialies == null)
                {
                    listSpecialies = SpecialityRepository.GetFakeSpecialities();
                    return listSpecialies;
                }
                return listSpecialies;
            }
            set
            {
                Set(ref listSpecialies, value);
            }

        }

        public Speciality()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
        }

        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить специальность?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (response.ToString() == "Yes")
                {
                    Speciality sp = (Speciality)p;
                    ListSpecialities.Remove(sp);
                }
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }

        }
    }
}
