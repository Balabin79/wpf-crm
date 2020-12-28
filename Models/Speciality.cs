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
    class Speciality : ViewModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }


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
