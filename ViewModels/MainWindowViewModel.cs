using System;
using System.Windows.Controls;
using Dental.Infrastructures.Commands.Base;
using Dental.Views.Pages;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Reflection;

namespace Dental.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private string _Title = "Dental";
        /// <summary>
        /// Страницы разделов программы
        /// </summary>
        private Page MainPage;
        private Page ShedulePage;
        private Page PatientListPage;
        private Page PriceListPage;
        private Page EmployeeListPage;
        private Page ReportListPage;
        private Page SettingsPage;

        private Page currentPage;
        private Double frameOpacity;



        public MainWindowViewModel()
        {
            FrameOpacity = 1;
            CurrentPage = new MainPage();

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            LeftMenuClick = new LambdaCommand(OnLeftMenuClickCommandExecuted, CanLeftMenuClickCommandExecute);
        }
        
        /// <summary>
        /// Текущая страница
        /// </summary>
        public Page CurrentPage
        {
            get => currentPage;
            set => Set(ref currentPage, value);
        }

        /// <summary>
        /// Используется для плавного переключения страниц разделов
        /// </summary>
        public Double FrameOpacity
        {
            get => frameOpacity;
            set => Set(ref frameOpacity, value);
        }

        /// <summary>
        /// Заголовок окна
        /// </summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }


        #region Command

        /// <summary>
        /// Команда закрытия приложения
        /// </summary>
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Команда переключения страниц через боковую (слева) панель меню (разделы)
        /// </summary>
        public ICommand LeftMenuClick { get; }
        private bool CanLeftMenuClickCommandExecute(object p) => true;
        private void OnLeftMenuClickCommandExecuted(object p)
        {
            bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            /* switch ((String)p)
             {
                 case "Main": Page main = this.Main ?? new Main(); SlowOpacity(main); break;
                 case "Shedule": Page shedule = this.Shedules ?? new Shedules(); SlowOpacity(shedule); break;
                 case "ClientsList": Page clientsList = this.ClientsList ?? new ClientsList(); SlowOpacity(clientsList); break;
                 case "PatientCard": Page patientCard = this.PatientCard ?? new PatientCard(isNew); SlowOpacity(patientCard); break;
                 case "Personals": Page personals = this.Personals ?? new Personals(); SlowOpacity(personals); break;
                 case "Prices": Page prices = this.Prices ?? new Prices(); SlowOpacity(prices); break;
                 case "Reports": Page reports = this.Reports ?? new Reports(); SlowOpacity(reports); break;
                 case "Settings": Page settings = this.Settings ?? new Settings(); SlowOpacity(settings); break;
                 default: Page def = this.Main ?? new Main(); SlowOpacity(def); break;
             }*/
            Page instance = (Page)Assembly.GetExecutingAssembly().CreateInstance(p.ToString());
            SlowOpacity(instance);
        }
        #endregion


        /// <summary>
        /// Обеспечивает плавное переключение разделов
        /// </summary>
        /// <param name="page"></param>
        private async void SlowOpacity(Page page)
        {
            await Task.Factory.StartNew(() => {
                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(20);
                }

                CurrentPage = page;
                for (double i = 0.0; i < 1.1; i += 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(20);
                }
            });
        }
    }
}
