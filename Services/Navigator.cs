using Dental.Infrastructures.Commands.Base;
using Dental.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity;
using Dental.Models;
using System.Collections.ObjectModel;


namespace Dental.Services
{
    /*
     * * Alexander Balabin
     * Навигация по программе
     * - поддержка истории просмотренных страниц
     * - поддержка отслеживание изменений на странице
     * - сохранение последней посещенной страницы
     * 
     * 
     * - добавить различные варианты анимации при переходах
     */
    public sealed class Navigator : ViewModelBase
    {
        public Navigator()
        {
            CurrentPage = GetStartPage();
            FrameOpacity = 1.1;
            LeftMenuClick = new LambdaCommand(OnLeftMenuClickCommandExecuted, CanLeftMenuClickCommandExecute);
            GoToHistoryItem = new LambdaCommand(OnGoToHistoryItemExecuted, CanGoToHistoryItemExecute);
        }

        public static Func<bool> HasUnsavedChanges { get; set; }
        public static Func<bool> UserSelectedBtnCancel { get; set; }

        // страница по умолчанию (стартовая страница, если не удалось подгрузить страницу из настроек)
        private readonly string defaultPage = "Dental.Views.Specialities";

        // коллекция просмотренных страниц
        public ObservableCollection<Links> BrowsingHistory { get; set; } = new ObservableCollection<Links>();

        public Page CurrentPage
        {
            get => currentPage;
            set => Set(ref currentPage, value);
        }
        public Double FrameOpacity
        {
            get => frameOpacity;
            set => Set(ref frameOpacity, value);
        }
        public int? StartWithLastPage { get; set; }

        public ICommand LeftMenuClick { get; }
        public ICommand GoToHistoryItem { get; }
        private bool CanLeftMenuClickCommandExecute(object p) => true;
        private bool CanGoToHistoryItemExecute(object p) => true;

        private void OnLeftMenuClickCommandExecuted(object p)
        {
            try
            {
                if (HasUnsavedChanges != null && UserSelectedBtnCancel != null)
                {
                    if (HasUnsavedChanges.Invoke() && UserSelectedBtnCancel.Invoke()) return;
                    HasUnsavedChanges = null; 
                    UserSelectedBtnCancel = null;
                }



                if (CurrentPage?.ToString() == "Dental.Views.Groups")
                {
                    if (((System.Windows.FrameworkElement)CurrentPage.Content).DataContext is ClientGroupViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;
                    }
                }

                if (CurrentPage?.ToString() == "Dental.Views.EmployeeGroups")
                {
                    if (((FrameworkElement)CurrentPage.Content).DataContext is EmployeeGroupViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;
                    }
                }

                if (CurrentPage?.ToString() == "Dental.Views.ClientsRequests")
                {
                    if (((FrameworkElement)CurrentPage.Content).DataContext is EmployeeGroupViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;
                    }
                }


                if (CurrentPage?.ToString() == "Dental.Views.Organization")
                {
                    if (((FrameworkElement)CurrentPage.Content).DataContext is OrganizationViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;

                    }
                }

                if (CurrentPage?.ToString() == "Dental.Views.Employee")
                {
                    if (CurrentPage?.DataContext is EmployeeViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;
                    }
                    /*if (((System.Windows.FrameworkElement)CurrentPage.Content).DataContext is EmployeeViewModel vm)
                    {
                        if (vm.HasUnsavedChanges() && vm.UserSelectedBtnCancel()) return;

                    }*/
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



                var links = new Links()
                {
                    Path = CurrentPage?.ToString(),
                    Id = 0,
                    Name = CurrentPage?.Title
                };
                BrowsingHistory.Add(links);
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
                var msg = "Не найден раздел";
            }

        }

        private void OnGoToHistoryItemExecuted(object p)
        {
            try
            {
                if (p is Links link)
                {
                    if (string.IsNullOrEmpty(link?.Path)) return;
                    LeftMenuClick?.Execute(link.Path);
                }
            } catch(Exception e)
            {

            }
        }

        public void LastPageSaving()
        {
            if (BrowsingHistory?.Count > 0)
            {
                using (var db = new ApplicationContext())
                {
                    try
                    {
                        if (StartWithLastPage == 1)
                        {
                            var settings = db.Settings.FirstOrDefault();
                            settings.StartPage = BrowsingHistory.Last().ToString();
                            db.SaveChanges();
                        }
                    }
                    catch { }
                }
            }
        }

        private Page CreatePage(string pageName, int param = -1)
        {
            Type type = Type.GetType(pageName);
            return (param == -1) ? (Page)Activator.CreateInstance(type) : (Page)Activator.CreateInstance(type, param);
        }

        private Page GetStartPage()
        {
            using (var db = new ApplicationContext())
            {
                try
                {
                    var settings = db.Settings.FirstOrDefault();
                    StartWithLastPage = settings?.StartWithLastPage ?? 0;
                    var startPage = db.Settings.FirstOrDefault()?.StartPage ?? defaultPage;
                    return CreatePage(startPage);
                }
                catch
                {
                    return CreatePage(defaultPage);
                }
            }
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

        public static Dictionary<string, string> SectionPage { get; } = new Dictionary<string, string>() {
            { "Advertising", "Рекламные источники" },
            { "Speciality", "Специальности сотрудников" },
        };

        private Page currentPage;
        private Double frameOpacity;

    }

    public class Links
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Time { get; set; } = DateTime.Now.ToShortTimeString();
    }


}
