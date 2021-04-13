using Dental.Infrastructures.Commands.Base;
using Dental.Views.HandbooksPages;
using Dental.Views.PatientCard;
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
            CurrentPage = new MainList();
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



        public BitmapImage OrganizationImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Nomenclature/org2.png"));
            }
        }



        public BitmapImage SpecialityImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Sections/Participation-Rate.png"));
            }
        }

        public BitmapImage SpecialityEmployeeImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Sections/Client-Account-Template.png"));
            }
        }

        public BitmapImage OrganizationEmployeeImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Sections/Reseller-Account.png"));
            }
        }

        public BitmapImage EmployeeImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Sections/Reseller-Programm.png"));
            }
        }



        public BitmapImage InsuranceCompanyImage
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Sections/License-Management.png"));
            }
        }




    }
}

