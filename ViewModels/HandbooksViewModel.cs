using Dental.Infrastructures.Commands.Base;
using Dental.Views.HandbooksPages;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Dental.ViewModels
{
    class HandbooksViewModel : ViewModelBase
    {
        public HandbooksViewModel()
        {
            FrameOpacity = 1;
            CurrentPage = new Employes();
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


        public ICommand LeftMenuClick { get; }
        private bool CanLeftMenuClickCommandExecute(object p) => true;
        private void OnLeftMenuClickCommandExecuted(object p)
        {
            //bool isNew = true; // это новая форма, т.е. нужно создать новые модели, а не загружать сущ-щие данные
            try
            {
                Page instance = CreatePage(p);
                SlowOpacity(instance);
            }
            catch (Exception e)
            {
                // записать в текстовой лог в каком месте возникла ошибка (название класса и строка) и e.Message
                int x = 0;
            }

        }


        private Page CreatePage(object page)
        {
            return (Page)Assembly.GetExecutingAssembly().CreateInstance(page.ToString());
        }


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

        private Page currentPage;
        private Double frameOpacity;

        public BitmapImage DiaryImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/Report.png"));
            }
        }

        public BitmapImage DiagnosImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/diagnos.png"));
            }
        }

        public BitmapImage PlanImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/plan.png"));
            }
        }

        public BitmapImage InitialImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Template/init.png"));
            }
        }

        public BitmapImage OrganizationImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/org2.png"));
            }
        }

        public BitmapImage   StatusEmployeeImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/status11.png"));
            }
        }

        public BitmapImage RoleImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Private.png"));
            }
        }

        public BitmapImage SpecialityImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Participation-Rate.png"));
            }
        }

        public BitmapImage EmployeeImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Reseller-Account.png"));
            }
        }

        public BitmapImage UsersImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/emp.png"));
            }
        }

        public BitmapImage AccessImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/comp.png"));
            }
        }

        public BitmapImage LoyalityImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Coins-In-Hand.png"));
            }
        }

        public BitmapImage AdvertisingImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Advertising.png"));
            }
        }

        public BitmapImage BonusImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Profiles.png"));
            }
        }

        public BitmapImage NewPatientImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/User-Add.png"));
            }
        }

        public BitmapImage ListPatientsImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/emp.png"));
            }
        }

        public BitmapImage SheduleImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/Shedule.png"));
            }
        }

    }
}

