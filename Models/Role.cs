using Dental.Infrastructures.Commands.Base;
using Dental.Repositories;
using Dental.ViewModels;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Input;

namespace Dental.Models
{
    class Role : ViewModelBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name="Наименование")]
        public string Name { get; set; }
        [Display(Description = "Описание")]
        public string Description { get; set; }
        public bool IsActive { get; set; }

        private ObservableCollection<Role> listRoles;       
        [NotMapped]
        public ObservableCollection<Role> ListRoles
        {
            get
            {
                if (listRoles == null)
                {
                    listRoles = RoleRepository.GetRoles();
                    return listRoles;
                }
                return listRoles;
            }
            set
            {
                Set(ref listRoles, value);
            }

        }

        public Role()
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
            }

        }

    }
}
