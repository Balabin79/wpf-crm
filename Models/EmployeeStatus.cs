using System.Windows.Input;
using Dental.Infrastructures.Commands.Base;
using System.Windows;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using Dental.ViewModels;
using Dental.Repositories;

namespace Dental.Models
{
    class EmployeeStatus : ViewModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }


        private ObservableCollection<EmployeeStatus> listEmployeeStatuses;
        public ObservableCollection<EmployeeStatus> ListEmployeeStatuses
        {
            get
            {
                if (listEmployeeStatuses == null)
                {
                    listEmployeeStatuses = EmployeeStatusRepository.GetFakeEmployeeStatuses();
                    return listEmployeeStatuses;
                }
                return listEmployeeStatuses;
            }
            set
            {
                Set(ref listEmployeeStatuses, value);
            }

        }

        public EmployeeStatus()
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
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить статус сотрудника?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (response.ToString() == "Yes")
                {
                    EmployeeStatus es = (EmployeeStatus)p;
                    listEmployeeStatuses.Remove(es);
                }
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }

        }
    }
}
