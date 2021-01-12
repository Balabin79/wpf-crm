﻿
using Dental.Interfaces;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Input;

namespace Dental.Models.Template
{
    [Table("TreatmentPlan")]
    class TreatmentPlan : ViewModelBase
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ParentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("Name")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Column("Dir")]
        [Display(Name = "Директория")]
        public int Dir { get; set; }

        //public ObservableCollection<DiagnosDetail> DiagnosesDetails { get; set; }





        /*
        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить роль?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (response.ToString() == "Yes")
                {
                    Role role = (Role)p;
                    RoleRepository.Delete(role);
                    //ListRoles.Remove(role);
                }
            }
            catch (Exception e)
            {

                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }}*/

    }
}
