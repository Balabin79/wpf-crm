using Dental.Infrastructures.Commands.Base;
using Dental.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Dental.Views.Pages.UserControls;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Data.Entity;
using Dental.Models;

namespace Dental.Services
{
    sealed class Navigation : ViewModelBase
    {
        private static readonly Navigation instance = new Navigation();

        static Navigation(){}
        private Navigation() { FrameOpacity = 1; LeftMenuClick = new LambdaCommand(OnLeftMenuClickCommandExecuted, CanLeftMenuClickCommandExecute); }

        public static Navigation Instance
        {
            get
            {
                return instance;
            }
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
            try
            {
                if (CurrentPage?.ToString() == "Dental.Views.Advertising")
                {
                    if (((System.Windows.FrameworkElement)CurrentPage.Content).DataContext is AdvertisingViewModel vm)
                    {
                       if(vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;
                    }
                }

                if (CurrentPage?.ToString() == "Dental.Views.Groups")
                {
                    if (((System.Windows.FrameworkElement)CurrentPage.Content).DataContext is ClientGroupViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;
                    }
                }



                //////////////////////////
                if (CurrentPage?.ToString() == "Dental.Views.PatientCard.MainInfoPage")
                {
                    var viewModel = CurrentPage.DataContext as PatientCardViewModel;
                    if (viewModel == null) return;
                    if (viewModel.HasUnsavedChanges())
                    {
                        bool response = viewModel.UserSelectedBtnCancel();
                        if (response) return;

                        if (viewModel.Model.Id != 0)
                        {
                            var model = Db.Instance.Context.PatientInfo.Find(viewModel.Model.Id);
                            if (model == null) return;
                            model = (PatientInfo)viewModel.ModelBeforeChanges.Copy(model);
                            Db.Instance.Context.Entry(model).State = EntityState.Modified;
                            Db.Instance.Context.SaveChanges();
                        }
                    }
                }
                if (CurrentPage?.ToString() == "Dental.Views.Organization")
                {
                    var viewModel = ((System.Windows.FrameworkElement)CurrentPage.Content).DataContext as OrganizationViewModel;
                    if (viewModel == null) return;
                    if (viewModel.HasUnsavedChanges())
                    {
                        bool response = viewModel.UserSelectedBtnCancel();
                        if (response) return;

                        if (viewModel.Model.Id != 0)
                        {
                            var model = Db.Instance.Context.Organizations.Find(viewModel.Model.Id);
                            if (model == null) return;
                            model = (Organization)viewModel.ModelBeforeChanges.Copy(model);
                            Db.Instance.Context.Entry(model).State = EntityState.Modified;
                            Db.Instance.Context.SaveChanges();
                        }
                    }
                }

                Page page;
                if (p is Array) 
                {
                    string pageName = (string)((object[])p)[0];
                    int id = (int)((object[])p)[1];
                    page = CreatePage(pageName, id);
                } 
                else page = CreatePage(p.ToString());
                SlowOpacity(page);
            }
            catch (Exception e)
            {
                int x = 0;
            }

        }

        private Page CreatePage(string pageName)
        {
            Type type = GetTypeByPageName(pageName);
            return (Page)Activator.CreateInstance(type);
        }

        private Page CreatePage(string pageName, int param)
        {
            Type type = GetTypeByPageName(pageName);
            return (Page)Activator.CreateInstance(type, param);
        }


        private Type GetTypeByPageName(string pageName)
        {
            return Type.GetType(pageName);
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

    }
}
