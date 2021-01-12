using Dental.Infrastructures.Commands.Base;
using Dental.Repositories;
using Dental.ViewModels;
using Dental.Views.HandbooksPages;
using DevExpress.Xpf.Core;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Dental.Models
{
    class Organization : ViewModelBase
    {
        //Общая инф-ция
        public string Logo { get; set; } // Логотип

        public string Inn { get; set; } // Инн
        public string Kpp { get; set; } // Кпп
        public string Name { get; set; } // Наименование орг-ции
        public string ShortName { get; set; } // Сокращение
        
        // Контактная инф-ция
        public string Address { get; set; } // Адрес
        public string Phone { get; set; }
        public string Email { get; set; }

        // Банковские реквизиты
        public string Bik { get; set; } // Бик
        public string AccountNumber { get; set; } // Расчетный счет
        public string BankName { get; set; } // Наименование

        // Регистрационная информация
        public string Сertificate { get; set; } // Свидетельство
        public string Ogrn { get; set; } // ОГРН                                      
        public string GeneralDirector { get; set; } // Генеральный директор
        public string License { get; set; } // Лицензия
        public string WhoIssuedBy { get; set; } // Кем выдана

        // Служебные поля
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        private ObservableCollection<Organization> listOrganizations;
        public ObservableCollection<Organization> ListOrganisations {
            get
            {
                if (listOrganizations == null)
                {
                    listOrganizations = OrganisationRepository.GetFakeOrganizations();
                    return listOrganizations;
                }
                return listOrganizations;
            }
            set
            {
                Set(ref listOrganizations, value);
            }
        }

        public Organization()
        {
            DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
            EditCommand = new LambdaCommand(OnEditCommandExecuted, CanEditCommandExecute);
            AddCommand = new LambdaCommand(OnAddCommandExecuted, CanAddCommandExecute);
        }


        public ICommand DeleteCommand { get; }
        private bool CanDeleteCommandExecute(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {             
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Вы уверены что хотите удалить организацию?", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
                if (response.ToString() == "Yes") {
                    Organization org = (Organization)p;
                    ListOrganisations.Remove(org);
                }
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }

        }

        public ICommand EditCommand { get; }
        private bool CanEditCommandExecute(object p) => true;
        private void OnEditCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {
                var response = ThemedMessageBox.Show(title: "Подтверждение действия", text: "Редактирование", messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Exclamation);
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }

        }

        public ICommand AddCommand { get; }
        private bool CanAddCommandExecute(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {
                //HandbooksViewModel vm = (HandbooksViewModel)Application.Current.Windows[0].Resources["viewModel"];
               // vm.LeftMenuClick.Execute((new NewOrganization()).GetType());
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
            }

        }

    }
}