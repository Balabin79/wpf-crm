using System.Collections.ObjectModel;
using Dental.ViewModels;
using Dental.Repositories;
using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using System.Windows;
using DevExpress.Xpf.Core;
using System;

namespace Dental.Models
{
    class Role : ViewModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        private ObservableCollection<Role> listRoles;
        public ObservableCollection<Role> ListRoles
        {
            get
            {
                if (listRoles == null)
                {
                    listRoles = RoleRepository.GetFakeRoles();
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
                    ListRoles.Remove(role);
                }
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }

        }

    }
}
